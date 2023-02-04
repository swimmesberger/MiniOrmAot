using System.Diagnostics;
using Microsoft.CodeAnalysis;

// ReSharper disable HeuristicUnreachableCode
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace MiniOrmAot.Analyzers; 

[Generator]
public class SourceGenerator : ISourceGenerator {
    public void Initialize(GeneratorInitializationContext context) {
#if DEBUG
        if (!Debugger.IsAttached) {
            // Debugger.Launch();
        }
#endif

        // Register a syntax receiver that will be created for each generation pass
        context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context) {
        // retrieve the populated receiver 
        if (context.SyntaxContextReceiver is SyntaxReceiver receiver == false) {
            return;
        }

        new Generator(context, receiver).Generate();
    }
}