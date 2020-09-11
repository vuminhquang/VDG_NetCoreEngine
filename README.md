# NetCore Addin Engine
NET Core engine with add-in availability

Howto use it:

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
