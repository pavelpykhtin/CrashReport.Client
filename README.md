# CrashReport.Client
This library provides adapter for .Net applications for catching and sending crash reports to the CrashReport logging system.

## Getting started
Add a nuget package to your dependencies:
```bash
PM> Install-Package CrashReport.Client
```
then configure nlog target
```xml
<configuration>
	<configSections>
		<section name="nlog" type="NLog.Config.ConfigSectionHandler, NLog"/>
	</configSections>
	
	<nlog>
		<extensions>
			<add assembly="CrashReport.Client" />
		</extensions>
		<targets>
            <target name="crashReport" type="CrashReport"
					layout="${Message}"
					VersionFromType="MyApplication.Program, MyApplication"
					ApplicationKey="your-application-key"
					Url="https://crashreport.collector.com" />
		</targets>
		<rules>
			<logger name="*" minlevel="Error" writeTo="crashReport" />
		</rules>
	</nlog>
</configuration>
```
Then you can write to nlog logger as usual:
```cs
NLog.Logger logger = LogManager.GetCurrentClassLogger();
_logger.Fatal(message);
```

## Configuration options:
|Option     |Required   |Description                                            |
|-----------|:---------:|-----------                                            |
|**Url**                |Yes        |Url of the reporting service               |
|**ApplicationKey**     |Yes        |Key that identifies your application in the system  |
|**Version**            |No         |Version of the application in the format 'major.minor.revision.build'  |
|**VersionFromType**    |No         |Full name of the type which is located in the assembly whose version should be used as version of the application. Usually it's the Application class (like program or MvcApplication). <br>This parameter is used if **Version** option was not specified. In case if both **Version** and **VersionFromType** were specified value of **Version** is used|
|**IsAsync**            |No         |Use async reporting. When enabled application will not wait until the message will be sent to the reporting service. Default value: **True**   |

## Tips:
### Putting additional data into the message
To add additional data to the message (like current user's Id) pass them as a last parameter of logging method:
```c#
logger.Fatal(e, message, new { UserId: 42 });
```
This will produce log message with AdditionalData property like this:
```json
{ "UserId": 42 }
```
Also you can pass multiple parameters without wrapping them into a single object. They will be serialized as an array:
```c#
logger.Fatal(e, message, 42, "John");
```
Will be serialized into:
```json
[42, "John"]
```
