﻿namespace CountryService.Web.Services;

public class ProtoService
{
    private readonly string _baseDirectory;

    public ProtoService(IWebHostEnvironment webHostEnvironment)
    {
        _baseDirectory = webHostEnvironment.ContentRootPath;
    }
    
    public Dictionary<string, IEnumerable<string?>> GetAll() =>
        Directory.GetDirectories($"{_baseDirectory}/protos")
            .Select(x => new
            {
                version = x, 
                protos = Directory.GetFiles(x).Select(Path.GetFileName) 
            })
            .ToDictionary(o => Path.GetRelativePath("protos",
                o.version), o => o.protos);
    
    public string Get(int version, string protoName)
    {
        var filePath = $"{_baseDirectory}/protos/v{version}/{protoName}";
        var exist = File.Exists(filePath);
        
        return exist ? filePath : string.Empty;
    }
    
    public async Task<string> ViewAsync(int version, string protoName)
    {
        var filePath = $"{_baseDirectory}/protos/v{version}/{protoName}";
        var exist = File.Exists(filePath);
        
        return exist ? await File.ReadAllTextAsync(filePath) : string.Empty;
    }
}