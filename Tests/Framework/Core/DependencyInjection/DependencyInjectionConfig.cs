using Autofac;
using Microsoft.Playwright;
using QuantumTestSuite.Framework.Core.Config;
using QuantumTestSuite.Framework.Core.Hooks;
using QuantumTestSuite.Framework.Core.Reporting;
using QuantumTestSuite.Framework.Core.Services;
using QuantumTestSuite.StepBindings;
using Reqnroll;
using Reqnroll.Autofac;

namespace QuantumTestSuite.Framework.Core.DependencyInjection;

/// <summary>
/// Configures dependency injection container for SpecFlow tests
/// </summary>
public class DependencyInjectionConfig
{
    [ScenarioDependencies]
    public static ContainerBuilder CreateContainerBuilder()
    {
        var builder = new ContainerBuilder();

        // Register configuration
        var settings = ConfigManager.Settings;
        builder.RegisterInstance(settings).SingleInstance();
        
        // Initialize AllureHelper with settings
        AllureHelper.Initialize(settings);

        // Register Playwright (per scenario)
        builder.Register(c => Microsoft.Playwright.Playwright.CreateAsync().GetAwaiter().GetResult())
            .As<IPlaywright>()
            .InstancePerLifetimeScope();

        // Register Services
        builder.RegisterType<BookingService>().As<IBookingService>().InstancePerLifetimeScope();
        builder.RegisterType<GitHubService>().As<IGitHubService>().InstancePerLifetimeScope();
        builder.RegisterType<HttpBinService>().As<IHttpBinService>().InstancePerLifetimeScope();

        // Register Hooks
        builder.RegisterType<TestHooks>().InstancePerLifetimeScope();

        // Register Step Bindings - Modular Organization
        // API Step Bindings
        builder.RegisterAssemblyTypes(typeof(DependencyInjectionConfig).Assembly)
            .Where(t => t.Namespace != null && t.Namespace.Contains("StepBindings.Api"))
            .InstancePerLifetimeScope();
        
        // UI Step Bindings  
        builder.RegisterAssemblyTypes(typeof(DependencyInjectionConfig).Assembly)
            .Where(t => t.Namespace != null && t.Namespace.Contains("StepBindings.Ui"))
            .InstancePerLifetimeScope();
        
        // Unit Step Bindings  
        builder.RegisterAssemblyTypes(typeof(DependencyInjectionConfig).Assembly)
            .Where(t => t.Namespace != null && t.Namespace.Contains("StepBindings.Unit"))
            .InstancePerLifetimeScope();

        return builder;
    }
}
