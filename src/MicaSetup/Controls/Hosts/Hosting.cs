﻿namespace MicaSetup.Controls;

public static class Hosting
{
    public static IHostBuilder CreateBuilder()
    {
        HostBuilder builder = new();
        return builder;
    }
}
