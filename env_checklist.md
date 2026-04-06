# CRM API Environment Variables Checklist

To successfully host your backend on **Render** or **Railway**, you must configure the following Environment Variables in their respective dashboards.

### 1. Database Connection (Critical)
- **Key**: `ConnectionStrings__NeonProductionDb`
- **Value**: `Host=ep-fancy-base-a1he613p-pooler.ap-southeast-1.aws.neon.tech;Database=neondb;Username=neondb_owner;Password=npg_a9ousdOS6mEZ;SSL Mode=Require;Trust Server Certificate=true`
> [!NOTE]
> This matches the connection string in your local `.env` file. Using double underscores (`__`) is the standard way to override nested `ConnectionStrings` in .NET.

### 2. Runtime Environment
- **Key**: `ASPNETCORE_ENVIRONMENT`
- **Value**: `Production`
> [!IMPORTANT]
> This tells .NET to use production security settings, including HTTPS redirection.

### 3. Networking
- **Key**: `PORT`
- **Value**: `8080`
> [!TIP]
> This must match the `EXPOSE 8080` in your `Dockerfile`. Render/Railway will route traffic to this port.

### 4. Logging & Diagnostics (Optional)
- **Key**: `LOGGING__LOGLEVEL__DEFAULT`
- **Value**: `Information`
