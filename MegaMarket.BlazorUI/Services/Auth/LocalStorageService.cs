using Microsoft.JSInterop;

namespace MegaMarket.BlazorUI.Services.Auth;

public class LocalStorageService
{
    private readonly IJSRuntime _jsRuntime;

    public LocalStorageService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task SetItemAsync(string key, string value)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorageHelper.setItem", key, value);
        }
        catch (InvalidOperationException)
        {
            // JS Interop not available (prerendering)
        }
    }

    public async Task<string?> GetItemAsync(string key)
    {
        try
        {
            return await _jsRuntime.InvokeAsync<string?>("localStorageHelper.getItem", key);
        }
        catch (InvalidOperationException)
        {
            // JS Interop not available (prerendering)
            return null;
        }
    }

    public async Task RemoveItemAsync(string key)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorageHelper.removeItem", key);
        }
        catch (InvalidOperationException)
        {
            // JS Interop not available (prerendering)
        }
    }

    public async Task ClearAsync()
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorageHelper.clear");
        }
        catch (InvalidOperationException)
        {
            // JS Interop not available (prerendering)
        }
    }
}
