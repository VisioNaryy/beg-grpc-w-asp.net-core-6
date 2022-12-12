﻿namespace CountryWiki.Domain.DTO;

public record CountryCreateDto
{
    public string Name { get; init; }
    public string Description { get; init; }
    public string FlagUri { get; init; }
    public string CapitalCity { get; init; }
    public string Anthem { get; init; }
    public IEnumerable<int> Languages { get; init; }
}