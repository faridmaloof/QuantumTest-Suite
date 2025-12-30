using Autofac;
using Microsoft.Playwright;
using QuantumTestSuite.Core.Config;
using QuantumTestSuite.Core.Hooks;
using QuantumTestSuite.Core.Reporting;
using QuantumTestSuite.Core.Services;
using QuantumTestSuite.Tests.StepBindings;
using QuantumTestSuite.Tests.StepBindings.UnitFeatures;
using Reqnroll;
using Reqnroll.Autofac;

namespace QuantumTestSuite.Core.DependencyInjection;

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
        
        // Unit Features Step Bindings
        builder.RegisterType<AbilitiesDemoStepBindings>().InstancePerLifetimeScope();
        builder.RegisterType<AbilitiesTestsStepBindings>().InstancePerLifetimeScope();
        builder.RegisterType<ConfigManagerStepBindings>().InstancePerLifetimeScope();
        builder.RegisterType<TestDataFactoryStepBindings>().InstancePerLifetimeScope();
        builder.RegisterType<UtilitiesStepBindings>().InstancePerLifetimeScope();

        return builder;
    }
}
