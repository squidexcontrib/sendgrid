![Squidex Logo](https://raw.githubusercontent.com/Squidex/squidex/master/media/logo-wide.png "Squidex")

# SendGrid Plugin for Squidex CMS

This plugin adds a rule action to Squidex to send out emails with SendGrid.

## How to install the Plugin?

1. Download the plugin from the release page https://github.com/squidexcontrib/sendgrid/releases/download/plugin1.0/sendgrid.zip
2. Extract the plugin to a folder, e.g. to `C:/Squidex/plugins/sendgrid`
3. Add the path to the plugin `C:/Squidex/plugins/sendgrid/publish/Squidex.Extensions.SendGrid.dll` to the configuration. Either...
    * Add it to the configuration file https://github.com/Squidex/squidex/blob/master/src/Squidex/appsettings.json#L24 OR
    * Add an environment variable `PLUGINS__1`

## How to install the Plugin with Docker?

Create a custom dockerfile where you download the plugin and add the environment.

```
FROM squidex/squidex:dev-2273

RUN apk update \
 && apk add --no-cache busybox \
 && apk add --no-cache wget

RUN mkdir -p plugins/sendgrid \
 && wget -qO- https://github.com/squidexcontrib/sendgrid/releases/download/plugin1.0/sendgrid.zip | busybox unzip -q -d plugins/sendgrid -

RUN ls /app/plugins/sendgrid

ENV PLUGINS__1 /app/plugins/sendgrid/publish/Squidex.Extensions.SendGrid.dll
```