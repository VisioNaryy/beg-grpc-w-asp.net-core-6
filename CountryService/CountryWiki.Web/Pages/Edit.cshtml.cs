using CountryWiki.Domain.DTO;
using CountryWiki.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CountryWiki.Web.Pages;

public class Edit : PageModel
{
    private readonly ICountryService _countryService;

    public string CountryName { get; set; }

    [BindProperty] public CountryUpdateDto CountryToUpdate { get; set; }

    public Edit(ICountryService countryService)
    {
        _countryService = countryService;
    }

    public async Task OnGetAsync(int id)
    {
        await RetrieveCountry(id);
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await RetrieveCountry(CountryToUpdate.Id);

            return Page();
        }

        await _countryService.UpdateAsync(new CountryUpdateDto
        {
            Id = CountryToUpdate.Id,
            Description = CountryToUpdate.Description
        });

        return RedirectToPage("./Index");
    }

    private async Task RetrieveCountry(int id)
    {
        var country = await _countryService.GetAsync(id);
        CountryName = country.Name;
        CountryToUpdate = new CountryUpdateDto
        {
            Id = country.Id,
            Description = country.Description
        };
    }
}