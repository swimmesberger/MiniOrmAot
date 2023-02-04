using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using MiniOrmAot.Analyzers.Model;

namespace MiniOrmAot.Analyzers; 

public class Generator {
    private readonly GeneratorExecutionContext _context;
    private readonly SyntaxReceiver _syntaxReceiver;
    private InitializationContext InitContext => _syntaxReceiver.InitializationContext;

    public Generator(GeneratorExecutionContext context, SyntaxReceiver syntaxReceiver) {
        _context = context;
        _syntaxReceiver = syntaxReceiver;
    }

    public void Generate() {
        try {
            GenerateDataMappers(_syntaxReceiver.GenerateMapperData);
        } catch (Exception e) {
            ReportError(e);
            throw;
        }
    }
        
    private void GenerateDataMappers(List<GenerateMapperData> mapperData) {
        var serviceExtensionsTemplateModel = new ServiceExtensionsTemplateModel();
        foreach (var data in mapperData) {
            foreach (var namedTypeSymbol in data.EntityTypes) {
                var rowMapper = GenerateRowMapper(namedTypeSymbol);
                serviceExtensionsTemplateModel.Mappers.Add(rowMapper);
            }
        }
        var sourceCode = InitContext.DataMapperServiceExtensionsTemplate.Render(serviceExtensionsTemplateModel, member => member.Name);
        _context.AddSource("DataRecordMapperServiceCollectionExtensions", SourceText.From(sourceCode, Encoding.UTF8));
    }
        
    private RowMapper GenerateRowMapper(GenerateMapperEntity entityType) {
        try {
            var templateModel = CreateModel(entityType);
            var sourceCode = InitContext.DataMapperTemplate.Render(templateModel, member => member.Name);
            var typeName = $"{templateModel.Entity.EntityName}DataRecordMapper";
            _context.AddSource(typeName, SourceText.From(sourceCode, Encoding.UTF8));
            return new RowMapper() {
                Name = typeName,
                EntityType = templateModel.Entity.TypeName,
                EntityNamespace = templateModel.Entity.Namespace
            };
        } catch (Exception e) {
            ReportError(e);
            throw;
        }
    }
        
    private RowMapperTemplateModel CreateModel(GenerateMapperEntity entityType) {
        return new RowMapperTemplateModel() {
            Entity = entityType
        };
    }
        
    private void ReportError(Exception e) {
        var descriptor = new DiagnosticDescriptor(nameof(SourceGenerator), "Error",
            $"{nameof(SourceGenerator)} failed to generate Interface due to an error. Please inform the author. Error: {e}",
            "Compilation", DiagnosticSeverity.Error, isEnabledByDefault: true);
        _context.ReportDiagnostic(Diagnostic.Create(descriptor, null));
    }
}