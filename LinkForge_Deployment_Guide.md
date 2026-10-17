# LinkForge Deployment Guide

> Markdown edition generated from the deployment guide PDF.

         LINKFORGE

         DEPLOYMENT GUIDE
         A structured record of the frontend + backend deployment




         React + Vite | ASP.NET Core Web API | SQL Server | IIS | Cloudflare Quick Tunnel




         Outcome
         The React frontend and ASP.NET Core API are served through one public HTTPS
         hostname. Link creation and short-link redirects were verified after correcting the IIS
         base-URL configuration.




         Prepared 16 October 2026

What this document covers Architecture, technology stack, folder
locations, IIS environment variables, Visual Studio publishing, Vite
build, static-file hosting, Cloudflare tunnel setup, validation
commands, troubleshooting, and production-readiness notes.

PUBLIC DEMO ADDRESS HOSTING STATUS

https://golden-players-guitar-mainstream.trycloudflare.com/ Windows
IIS + ASP.NET Core Frontend and API verified

Note: The address is a Cloudflare Quick Tunnel URL. It is temporary and
suitable for a demo, not a stable permanent production domain. LINKFORGE
\| DEPLOYMENT GUIDE

1.  Architecture at a glance The frontend and API share one public
    hostname. IIS serves the React static assets and runs the ASP.NET
    Core API. SQL Server stores short-link data.

        Browser
        Public HTTPS URL

        ↓

        Cloudflare Quick Tunnel
        Public HTTPS → local IIS origin

        ↓ forwards to http://localhost:8081

        IIS site: LinkForge
        Physical root: C:\Deployments\LinkForge\Api

        / → React files in wwwroot
        /api/Links → API controller
        /{shortCode} → short-link redirect

        ↓ EF Core / Microsoft.Data.SqlClient

        SQL Server
        Database: LinkForgeDb

Why same-origin API paths matter The frontend HTTP client combines
appConfig.apiBaseUrl with each API path. With VITE_API_BASE_URL empty
and paths such as /api/Links, the browser calls the same hostname as the
frontend. This avoids embedding localhost into the production JavaScript
bundle.

2.  Technology stack Layer Technology Purpose

    Frontend React 18.3.1 User interface for creating short links

    Build Vite 6.x + npm Builds static assets into dist/

    Icons lucide-react Frontend icon components

    Backend ASP.NET Core Web API, .NET 10 API endpoints, validation, URL
    creation and redirects

    Architecture Clean Architecture Api, Application, Domain,
    Infrastructure projects

    ORM/provider EF Core SQL Server 10.0.12 Data access from
    Infrastructure

    SQL driver Microsoft.Data.SqlClient 6.1.6 SQL Server connection

    Database Microsoft SQL Server Stores short-link records

    Web server Windows IIS 10 + ASP.NET Core Hosting Serves static files
    and runs API via AspNetCoreModuleV2 Bundle

    Public ingress Cloudflare Quick Tunnel / cloudflared Temporary
    public HTTPS access to local IIS

    Diagnostics PowerShell, curl.exe, Event Viewer Endpoint verification
    and server-error diagnosis

Deployment record • 16 October 2026 Page 2 LINKFORGE \| DEPLOYMENT GUIDE

3.  Folders and environment configuration Item Path / value

    Backend source
    C:`\P`{=tex}2`\Linkforge`{=tex}`\Linkforge`{=tex}-backend

    Frontend source
    C:`\P`{=tex}2`\Linkforge`{=tex}`\LinkForge`{=tex}-Frontend

    Vite build output
    C:`\P`{=tex}2`\Linkforge`{=tex}`\LinkForge`{=tex}-Frontend`\dist`{=tex}

    Frontend staging
    C:`\Deployments`{=tex}`\LinkForge`{=tex}`\Frontend`{=tex}

    IIS root / published API
    C:`\Deployments`{=tex}`\LinkForge`{=tex}`\Api`{=tex}

    Frontend files served by API
    C:`\Deployments`{=tex}`\LinkForge`{=tex}`\Api`{=tex}`\wwwroot`{=tex}

    Database LinkForgeDb on QGINDELT37

    IIS site / local origin LinkForge / HTTP port 8081

    Public demo URL
    https://golden-players-guitar-mainstream.trycloudflare.com/

IIS environment variables Configured in IIS Manager → LinkForge →
Configuration Editor → system.webServer/aspNetCore →
environmentVariables:

       Name                                      Expected value / purpose

       ASPNETCORE_ENVIRONMENT                    Production

       LinkForge__BaseUrl                        https://golden-players-guitar-mainstream.trycloudflare.com

       ConnectionStrings__LinkForgeDb            SQL Server connection string for LinkForgeDb; deployed setup used Windows
                                                 Integrated Authentication, Encrypt=True and TrustServerCertificate=True.

Important: After publishing, the API inserted a row but returned HTTP
500 because LinkForge:BaseUrl was not configured for the running IIS
process. Restoring LinkForge\_\_BaseUrl and recycling the application
pool fixed link creation.

Deployment record • 16 October 2026 Page 3 LINKFORGE \| DEPLOYMENT GUIDE

4.  Deployment steps The sequence below reflects the deployment
    performed. Each command set is placed in its own labeled panel for
    easy copying and reference.

Step 1 --- Publish the backend in Visual Studio Open the backend
solution, right-click LinkForge.Api → Publish, choose the existing
Folder profile, select Release and net10.0, and publish to
C:`\Deployments`{=tex}`\LinkForge`{=tex}`\Api`{=tex}. Verify
LinkForge.Api.dll and web.config are present.

Step 2 --- Configure IIS and database access IIS and the ASP.NET Core
Hosting Bundle were installed. The LinkForge site uses
C:`\Deployments`{=tex}`\LinkForge`{=tex}`\Api `{=tex}as its physical
root and listens locally on port 8081. The IIS application-pool identity
was granted the database permissions needed for LinkForgeDb.

Step 3 --- Configure the required IIS environment variables • Set
ASPNETCORE_ENVIRONMENT to Production.

• Set LinkForge\_\_BaseUrl to the current public HTTPS tunnel URL.

• Set ConnectionStrings\_\_LinkForgeDb to the database connection
string.

• Click Apply and recycle the application pool after changes.

Step 4 --- Start the public tunnel COMMAND PANEL 1 --- Start Cloudflare
Quick Tunnel (PowerShell)

       cloudflared tunnel --url http://localhost:8081

Keep the cloudflared terminal open. The assigned trycloudflare.com
hostname may change when the tunnel is restarted.

Step 5 --- Configure the frontend for same-origin API requests In
src/config/env.js, the localhost fallback was removed so an empty API
base URL remains empty. In the frontend .env file, set VITE_API_BASE_URL
to an empty value:

      CONFIG PANEL 2 — Frontend .env

       VITE_API_BASE_URL=

The frontend HTTP client can then request paths such as /api/Links on
the current origin.

Step 6 --- Build the React frontend COMMAND PANEL 3 --- Build frontend
(PowerShell)

       cd C:\P2\Linkforge\LinkForge-Frontend
       npm run build

Vite generated dist/index.html and compiled assets under dist/assets/.

Deployment record • 16 October 2026 Page 4 LINKFORGE \| DEPLOYMENT GUIDE

5.  Deploy static files and verify The frontend was hosted inside the
    existing ASP.NET Core IIS application so the frontend and API could
    share one hostname.

Step 7 --- Create the wwwroot folder COMMAND PANEL 4 --- Create frontend
web root (PowerShell)

       New-Item -ItemType Directory `
         -Path "C:\Deployments\LinkForge\Api\wwwroot" `
         -Force

Step 8 --- Copy the Vite build into wwwroot COMMAND PANEL 5 --- Copy
built frontend files (PowerShell)

       Copy-Item `
         "C:\Deployments\LinkForge\Frontend\*" `
         "C:\Deployments\LinkForge\Api\wwwroot\" `
         -Recurse -Force

Step 9 --- Enable static-file middleware In Program.cs, the following
lines were added before app.MapControllers():

      CODE PANEL 6 — Program.cs

       app.UseDefaultFiles();
       app.UseStaticFiles();

Publish LinkForge.Api again using the Visual Studio folder profile.
Because publishing can replace or omit manually copied assets, copy the
frontend files again if needed.

Step 10 --- Verify that index.html exists COMMAND PANEL 7 --- Verify
frontend files (PowerShell)

       Test-Path "C:\Deployments\LinkForge\Api\wwwroot\index.html"

Expected output: True.

Step 11 --- Test public redirect COMMAND PANEL 8 --- Check redirect
headers (PowerShell)

       curl.exe -sS -D - -o NUL --max-time 30 `
         "https://&lt;current-tunnel-host&gt;/&lt;short-code&gt;"

A working short link returned HTTP 302 Found with a Location header
pointing to the original destination.

Deployment record • 16 October 2026 Page 5 LINKFORGE \| DEPLOYMENT GUIDE

6.  Verification results Check Observed result Meaning

    Public root URL React UI loaded Frontend static files served by IIS

    POST /api/Links Success response with shortUrl and expiresAt Link
    creation works after BaseUrl was restored

    GET /KF9kBD7 302 Found with expected Location header Short-link
    redirect works

    /swagger/index.html in 404 Swagger middleware is Development-only
    Production

    Database check during error Record was inserted although API
    returned 500 Failure was after insert while creating response URL

Example POST test with a JSON file A JSON file was used for
PowerShell/curl testing to avoid shell-quoting errors.

      COMMAND PANEL 9 — Create request JSON file (PowerShell)

       $jsonPath = "$env:TEMP\linkforge-test.json"
       @'
       {
          "originalUrl": "https://www.wikipedia.org",
          "customAlias": null
       }
       '@ | Set-Content -Path $jsonPath -Encoding ascii


      COMMAND PANEL 10 — Send the JSON file (PowerShell)

       curl.exe -i -X POST `
         "https://&lt;current-tunnel-host&gt;/api/Links" `
         -H "Content-Type: application/json" `
         --data-binary "@$jsonPath"

The test above was used to isolate the server-side 500 error. Event
Viewer identified the missing LinkForge:BaseUrl setting; after restoring
the IIS environment variable, link creation worked.

Deployment record • 16 October 2026 Page 6 LINKFORGE \| DEPLOYMENT GUIDE

7.  Troubleshooting record Root URL returned 404 before frontend setup
    The API did not originally have a frontend root page. Copying the
    Vite build to wwwroot and enabling default/static-file middleware
    made the React UI available at /.

Swagger returned 404 in Production Expected behavior: Program.cs
registers Swagger only inside if (app.Environment.IsDevelopment()).

Short-link creation returned HTTP 500 while a row appeared in SQL Server
Event Viewer reported System.InvalidOperationException:
LinkForge:BaseUrl is not configured. Restore LinkForge\_\_BaseUrl in IIS
Configuration Editor and recycle the app pool.

curl request returned a JSON parsing 400 PowerShell quoting produced
malformed JSON. Writing the body to a file and sending it with
--data-binary avoided command-line escaping issues.

Frontend loaded but API request failed Test POST /api/Links directly. If
the backend returns 500, inspect Windows Logs → Application in Event
Viewer. If direct API succeeds, inspect frontend request paths and
browser developer tools.

Public URL stopped working Quick Tunnel depends on the cloudflared
process. Keep it running; if restarting creates a new hostname, update
LinkForge\_\_BaseUrl and verify the frontend still uses relative paths.

8.  Production-readiness notes The deployment works as a public demo,
    but the current setup should not be described as a permanent
    production service.

• Temporary URL: Cloudflare Quick Tunnel has no uptime guarantee and the
hostname can change. Use a stable domain and durable hosting/networking
for real users.

• NuGet security warning: The audit identified
System.Security.Cryptography.Pkcs 9.0.11 with moderate and high severity
advisories. Dependency chain: EF Core SQL Server 10.0.12 →
Microsoft.Data.SqlClient 6.1.6 → System.Security.Cryptography.Pkcs
9.0.11. Review a compatible patched version before real production use.

• Secrets: Keep database connection strings and credentials out of
source control. Verify that the IIS app-pool identity has only required
permissions.

• Origin: Public requests use HTTPS at Cloudflare and are forwarded to
local IIS over HTTP port 8081 in this tested setup. Review the network
and origin security model before permanent hosting.

• Operations: Add monitoring, structured logs, database backups and
recovery procedures before serving real users.

• Production environment: Keep ASPNETCORE_ENVIRONMENT=Production. Do not
expose Development-mode Swagger publicly.

• Repeatable deployment: After publishing the API, verify that
wwwroot/index.html and assets are still present. Consider automating the
build/copy/publish sequence.

Deployment summary Component Deployed arrangement

       Frontend                       React/Vite build served from API application's wwwroot

       Backend                        ASP.NET Core Web API on .NET 10, hosted by IIS

Deployment record • 16 October 2026 Page 7 LINKFORGE \| DEPLOYMENT GUIDE

       Component                       Deployed arrangement

       Database                        SQL Server database LinkForgeDb

       Ingress                         Cloudflare Quick Tunnel to localhost:8081

       Routing                         Single hostname for frontend, /api/Links and short-code redirects

       Status                          Public demo verified; additional hardening needed for permanent production

Prepared from the LinkForge deployment session dated 16 October 2026.
Local paths and the tunnel URL are environment-specific; update them if
the host or tunnel changes.

Deployment record • 16 October 2026 Page 8 
