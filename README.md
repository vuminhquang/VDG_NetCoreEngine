# NetCore Addin Engine
NET Core engine with add-in availability

Howto use it:

Important: Create the folder AddIns within your client project (.net core console  OR ASP.NET are fine)


From
```
public static IHostBuilder CreateHostBuilder(string[] args) =>
	Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.UseStartup<Startup>();
    });
```

To
```	
public static IHostBuilder CreateHostBuilder(string[] args) =>
	Global.CreateHostBuilder(args)
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.UseStartup<Startup>();
    });
```	
