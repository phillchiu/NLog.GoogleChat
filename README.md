NLog.GoogleChat
==========
An NLog target for Google Chat

Usage
=====
1. Create a Google Chat WebhookUrl
2. Configure NLog to use `NLog.GoogleChat`:

### NLog.config

```xml
<?xml version="1.0" encoding="utf-8" ?>
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
      xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
	    autoReload="true"
      internalLogLevel="Off"
      internalLogFile="c:\temp\internal-nlog.txt">

	<extensions>
		<add assembly="NLog.GoogleChat"/>
	</extensions>

	<targets async="true">
		<!--WebhookUrl ex:https://chat.googleapis.com/v1/spaces/xxxxxxxx/messages?key=xxxxxxxxxxxx&amp;token=xxxxxxxx-->
		<target xsi:type="GoogleChat"
					name="googleChat"
					WebhookUrl="xxx"
					layout="*${level:uppercase=true}*: ${longdate} | ${event-properties:item=Environment} | ${event-properties:item=Application:whenEmpty=MyApp} | ${event-properties:item=UserId} ${newline}${logger} : ${message}${newline}${exception:format=tostring}"/>
	</targets>
	<rules>
		<logger name="*" levels="Info,Error" writeTo="googleChat" />
	</rules>
</nlog>
```


### Configuration Options

Key        | Description
----------:| -----------
WebhookUrl    | Your WebhookUrl (https://chat.googleapis.com/v1/spaces/xxxxxxxx/messages?key=xxxxxxxxxxxx&amp;token=xxxxxxxx)
