# Social CRM Office SCRM (Social CRM) 

Social Office is a set free and open source tools (mainly in spanish) for Social Media Marketing, Digital Marketing, Social Managers, Community Managers, 
by [Javier Cañon](https://www.javiercanon.com) 
 

## What does it Do? ##

Social CRM has applications in marketing, customer service and sales, including:

*    Social Engagement with prospects: SocialCRM tools allow businesses to better engage with their customers by, for example, listening to sentiments about their products and services.
*    Social Customer Service: Ownership of social media is shifting away from Marketing and Communication as engagement increasingly relates to inbound customer service-based topics. Rather than social being seen purely as a space for companies to deliver outbound marketing messages, it is the inbound customer queries that allow for meaningful points of engagement and the building of brand advocacy.
*    Personalized Marketing Strategy: The ability to create custom content is increasingly dependent on access to reliable, qualitative social user data to facilitate precise audience segmentation. Furthermore, dynamic audience segments, built on both social data and demographic data, allow for more accurate measurement of campaign KPIs.

Traditional customer relationship management focuses on collecting and managing static customer data, such as past purchase information, contact history and customer demographics. 
This information is often sourced from email and phone interactions, commonly limited to direct interactions between the company and the customer.
Social CRM adds a deeper layer of information onto traditional CRM by adding data derived from social networks like Facebook, Twitter, LinkedIn or any other social network where a user publicly shares information. 
The key benefit of social CRM is the ability for companies to interact with customers in a multichannel retailing environment (commonly referred to as omnichannel) and talk to customers the way they talk to each other. 
Social CRM enables companies to track a customer's social influence and source data from conversations occurring outside of formal, direct communication. 
Social CRM also allows companies to keep a full audit history of all customer interactions, regardless of social channel they choose to use, available to all customer care employees.

### Social CRM metrics in applications

Metrics for building awareness:
* web traffic
* search volume trends
* volume of followers
* social mentions

Metrics for increasing sales:

* website traffic
* social mentions
* volume of followers
* repeat visits
* social content acceptance rate

Metrics for assessing changes environment in an industry:
* Share of Voice (how much of the overall voice a single brand consumes)


### Screenshots

![](docs/img/screenshoot1.png?raw=true)


## Philosophy
* KISS by design and programming. An acronym for "keep it simple, stupid" or "keep it stupid simple", is a design principle. The KISS principle states that most systems work best if they are kept simple rather than made complicated; therefore, simplicity should be a key goal in design, and unnecessary complexity should be avoided. Variations on the phrase include: "Keep it simple, silly", "keep it short and simple", "keep it simple and straightforward", "keep it small and simple", or "keep it stupid simple".

* Select the best tools for the job, use tools that take less time to finish the job.
* Productivity over complexity and avoid unnecessary complexity for elegant or beauty code.

* Computers are machines, more powerful every year, give them hard work, concentrate on being productive.

### Community 

* [Website](https://javiercanon.github.io/Social-Office-SCRM/)
* [Group - Community](https://www.facebook.com/groups/socialoffice/)
* [Wiki](https://github.com/JavierCanon/Social-Office-SCRM/wiki)
* [Issue - Bug Traking - Feature Request](https://github.com/JavierCanon/Social-Office-SCRM/issues)

## Demo Installers 

### Standalone Windows 10

For use only in one computer/machine.
Minimum recommended standalone machine configuration:
* Windows 10 64 bit.
* 8 Gigas RAM.
* [CPU Intel Core I3](https://www.intel.com/content/www/us/en/products/processors/core/i3-processors.html) or AMD equivalent.
* 50 Gigas of free space.

![Attention](https://assets-cdn.github.com/images/icons/emoji/unicode/26a0.png)
* Requires [.Net Framework 4.7.2](https://dotnet.microsoft.com/download/thank-you/net472). 
* Requires [IIS 10 Express 64 bit](https://www.microsoft.com/en-us/download/details.aspx?id=48264). 
* Requires [SQL Server 2017 Express](https://www.microsoft.com/en-us/download/details.aspx?id=55994) [1]. 

[1] Limitations : Microsoft SQL Server Express supports 1 physical processor, 1 GB memory, and 10 GB storage.

* [Download Standalone Windows 10 64x Installers](https://github.com/JavierCanon/Social-Office-SCRM/releases)

### Windows Server 2012R2 and Up

For multiuser, client/server using LAN network or Internet.
Minimum recommended server machine configuration:
* 8 Gigas RAM.
* 4 CPU Cores in virtual servers.
* CPU XEON or Intel Core I7 in dedicated servers.
* 50 Gigas of free space.

Minimum recommended clients machine configuration:
* Windows 10 64 bit.
* 8 Gigas RAM.
* [CPU Intel Core I3](https://www.intel.com/content/www/us/en/products/processors/core/i3-processors.html) or AMD equivalent.
* 50 Gigas of free space.

![Attention](https://assets-cdn.github.com/images/icons/emoji/unicode/26a0.png)
* Requires [.Net Framework 4.7.2](https://dotnet.microsoft.com/download/thank-you/net472). 
* Requires IIS 10 enabled in server. 
* Requires [SQL Server 2017 Web, Standard, Datacenter or Enterprise](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) [1]. 

[1] [Compare SQL Server 2017 Editions](https://docs.microsoft.com/en-us/sql/sql-server/editions-and-components-of-sql-server-2017?view=sql-server-2017).

* No downloads yet.

## Getting Started

### Prerequisites

Things that you need to install the software and how to install them

* Windows 10 64 bits (may works on 7.x and 8.x 64 bits but not tested).
* .Net Framework 4.7.2.
* Sql Server 2017 (any version).
* IIS 10 (Internet Information Server).

### Installing

A step by step series of examples that tell you how to get a development env running

Say what the step will be

```
Give the example
```

And repeat

```
until finished
```

End with an example of getting some data out of the system or using it for a little demo


### Running the tests

Explain how to run the automated tests for this system

### Break down into end to end tests

Explain what these tests test and why

```
Give an example
```

### And coding style tests

#### Coding Offical Reference

1. [MSDN General Naming Conventions](http://msdn.microsoft.com/en-us/library/ms229045(v=vs.110).aspx)
2. [DoFactory C# Coding Standards and Naming Conventions](http://www.dofactory.com/reference/csharp-coding-standards) 
3. [MSDN Naming Guidelines](http://msdn.microsoft.com/en-us/library/xzf533w0%28v=vs.71%29.aspx)
4. [MSDN Framework Design Guidelines](http://msdn.microsoft.com/en-us/library/ms229042.aspx)

#### Model 
[MVP](https://en.wikipedia.org/wiki/Model%E2%80%93view%E2%80%93presenter) 

```
First, anything that a user can interact with, or just be shown, is a view. The laws, behavior and characteristics of such a view is described by an interface. That interface can be implemented using a WinForms UI, a console UI, a web UI or even no UI at all (usually when testing a presenter) - the concrete implementation just doesn't matter as long as it obeys the laws of its view interface.

Second, a view is always controlled by a presenter. The laws, behavior and characteristics of such a presenter is also described by an interface. That interface has no interest in the concrete view implementation as long as it obeys the laws of its view interface.

Third, since a presenter controls its view, to minimize dependencies there's really no gain in having the view knowing anything at all about its presenter. There's an agreed contract between the presenter and the view and that's stated by the view interface.

The implications of Third are:

    The presenter doesn't have any methods that the view can call, but the view has events that the presenter can subscribe to.
    The presenter knows its view. I prefer to accomplish this with constructor injection on the concrete presenter.
    The view has no idea what presenter is controlling it; it'll just never be provided any presenter.

```

### Deployment

Add additional notes about how to deploy this on a live system

### Build Dependencies 

* [Devexpress 18.2.3](https://go.devexpress.com/devexpressdownload_universaltrial.aspx)
* [Cefsharp](https://github.com/cefsharp/CefSharp)
* [SharkErrorReporter](https://github.com/JavierCanon/Shark.NET-Error-Reporter)

## Authors 

* Social Office by Javier Cañon [EN](https://www.javiercanon.com) [ES](https://www.javiercañon.com)
* Used third party tools, propietary and open source.

## Bugs 
Please submit [bug reports](https://github.com/JavierCanon/Social-Office-SCRM/issues) or feature requests on GitHub.


## Roadmap 

* [Check this link](/ROADMAP.md)


## Supported by, thanks to 

* [Onlyoffice](https://www.onlyoffice.com/) 
* [SplendidCRM](https://www.splendidcrm.com/) 
--
![Caphyon](https://raw.githubusercontent.com/JavierCanon/Social-Office-Browser/master/docs/img/advanced-installer-iconNavLogo.png)
* [Caphyon Advanced Installer](https://www.advancedinstaller.com) 
--
![Softcanon](https://github.com/JavierCanon/Social-Office-Webackeitor/raw/master/docs/images/logo_softcanon_200x75.gif) 
* [Softcanon](https://www.softcanon.com)
--

## License

This project is licensed under the GNU AFFERO GENERAL PUBLIC LICENSE Version 3 - see the [LICENSE.md](/LICENSE.md) file for details.

---
Made with ❤️ by **[Javier Cañon](https://www.javiercanon.com)**.