/*
 *
 * (c) Copyright Ascensio System Limited 2010-2020
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Configuration;
using ASC.Common.Logging;
using ASC.Web.Core.Utility;
using ASC.Web.Core.WebZones;

namespace ASC.Web.Core
{
    [Flags]
    public enum ItemAvailableState
    {
        Normal = 1,
        Disabled = 2,
        All = Normal | Disabled
    }

    public class WebItemManager
    {
        private static readonly ILog log = LogManager.GetLogger("ASC.Web");

        private readonly Dictionary<Guid, IWebItem> items = new Dictionary<Guid, IWebItem>();
        private static readonly string disableItem = WebConfigurationManager.AppSettings["web.disabled-items"] + ",";


        public static Guid CommunityProductID
        {
            get { return new Guid("{EA942538-E68E-4907-9394-035336EE0BA8}"); }
        }
        
        public static Guid ProjectsProductID
        {
            get { return new Guid("{1e044602-43b5-4d79-82f3-fd6208a11960}"); }
        }

        public static Guid CRMProductID
        {
            get { return new Guid("{6743007C-6F95-4d20-8C88-A8601CE5E76D}"); }
        }

        public static Guid DocumentsProductID
        {
            get { return new Guid("{E67BE73D-F9AE-4ce1-8FEC-1880CB518CB4}"); }
        }

        public static Guid PeopleProductID
        {
            get { return new Guid("{F4D98AFD-D336-4332-8778-3C6945C81EA0}"); }
        }

        public static Guid MailProductID
        {
            get { return new Guid("{2A923037-8B2D-487b-9A22-5AC0918ACF3F}"); }
        }

        public static Guid CalendarProductID
        {
            get { return new Guid("{32D24CB5-7ECE-4606-9C94-19216BA42086}"); }
        }

        public static Guid BirthdaysProductID
        {
            get { return new Guid("{37620AE5-C40B-45ce-855A-39DD7D76A1FA}"); }
        }

        public static Guid TalkProductID
        {
            get { return new Guid("{BF88953E-3C43-4850-A3FB-B1E43AD53A3E}"); }
        }

        public static Guid VoipModuleID
        {
            get { return new Guid("{46CFA73A-F320-46CF-8D5B-CD82E1D67F26}"); }
        }

        public static Guid SampleProductID
        {
            get { return new Guid("{314B5C27-631B-4C6C-8B11-C6400491ABEF}"); }
        }

        public static WebItemManager Instance { get; private set; }

        public IWebItem this[Guid id]
        {
            get
            {
                IWebItem i;
                items.TryGetValue(id, out i);
                return i;
            }
        }

        static WebItemManager()
        {
            Instance = new WebItemManager();
        }

        private WebItemManager()
        {
        }

        public void LoadItems()
        {
            foreach (var file in GetFiles())
            {
                try
                {
                    if (EnabledWebItem(file))
                    {
                        var webitems = LoadWebItem(file);
                        foreach (var webitem in webitems)
                        {
                            if (RegistryItem(webitem))
                            {
                                log.DebugFormat("Web item {0} loaded", webitem.Name);
                            }
                        }
                    }
                }
                catch (Exception exc)
                {
                    log.Error(string.Format("Couldn't load web item {0}", file), exc);
                }
            }
        }

        public bool RegistryItem(IWebItem webitem)
        {
            lock (items)
            {
                if (webitem != null && this[webitem.ID] == null)
                {
                    if (webitem is IAddon)
                    {
                        ((IAddon)webitem).Init();
                    }
                    if (webitem is IProduct)
                    {
                        ((IProduct)webitem).Init();
                    }
                    if (webitem is IModule)
                    {
                        var module = (IModule)webitem;
                        if (module.Context != null && module.Context.SearchHandler != null)
                        {
                            SearchHandlerManager.Registry(module.Context.SearchHandler);
                        }
                    }

                    items.Add(webitem.ID, webitem);
                    return true;
                }
                return false;
            }
        }

        public List<IWebItem> GetItems(WebZoneType webZone)
        {
            return GetItems(webZone, ItemAvailableState.Normal);
        }

        public List<IWebItem> GetItems(WebZoneType webZone, ItemAvailableState avaliableState)
        {
            var copy = items.Values.ToList();
            var list = copy.Where(item =>
                {
                    if ((avaliableState & ItemAvailableState.Disabled) != ItemAvailableState.Disabled && item.IsDisabled())
                    {
                        return false;
                    }
                    var attribute = (WebZoneAttribute)Attribute.GetCustomAttribute(item.GetType(), typeof(WebZoneAttribute), true);
                    return attribute != null && (attribute.Type & webZone) != 0;
                }).ToList();

            list.Sort((x, y) => GetSortOrder(x).CompareTo(GetSortOrder(y)));
            return list;
        }

        public List<IWebItem> GetSubItems(Guid parentItemID)
        {
            return GetSubItems(parentItemID, ItemAvailableState.Normal);
        }

        public List<IWebItem> GetSubItems(Guid parentItemID, ItemAvailableState avaliableState)
        {
            return GetItems(WebZoneType.All, avaliableState).OfType<IModule>()
                                                            .Where(p => p.ProjectId == parentItemID)
                                                            .Cast<IWebItem>()
                                                            .ToList();
        }

        public Guid GetParentItemID(Guid itemID)
        {
            var m = this[itemID] as IModule;
            return m != null ? m.ProjectId : Guid.Empty;
        }

        public static int GetSortOrder(IWebItem item)
        {
            return item != null && item.Context != null ? item.Context.DefaultSortOrder : 0;
        }

        public List<IWebItem> GetItemsAll()
        {
            var list = items.Values.ToList();
            list.Sort((x, y) => GetSortOrder(x).CompareTo(GetSortOrder(y)));
            return list;
        }

        public List<T> GetItemsAll<T>() where T : IWebItem
        {
            return GetItemsAll().OfType<T>().ToList();
        }

        private IEnumerable<IWebItem> LoadWebItem(string file)
        {
            var assembly = Assembly.LoadFrom(file);
            var attributes = assembly.GetCustomAttributes(typeof(ProductAttribute), false).Cast<ProductAttribute>();
            return attributes.Where(r=> r != null).Select(productAttribute => (IWebItem)Activator.CreateInstance(productAttribute.Type));
        }

        private IEnumerable<string> GetFiles()
        {
            const string pattern = "ASC.Web.*.dll";
            if (HttpContext.Current != null)
            {
                return Directory.GetFiles(HttpContext.Current.Server.MapPath("~/bin"), pattern);
            }
            return Directory.GetFiles(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), pattern);
        }

        private bool EnabledWebItem(string file)
        {
            var parts = file.Split('.');
            var name = 1 < parts.Length ? parts[parts.Length - 2] + "," : string.Empty;
            return disableItem.IndexOf(name, StringComparison.InvariantCultureIgnoreCase) == -1;
        }
    }
}