﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Threading.Tasks;

namespace Append.Blazor.Sidepanel
{
    public partial class Sidepanel : IDisposable
    {
        private ElementReference _element;

        [Inject] public ISidepanelService Service { get; set; } = default!;
        [Inject] public NavigationManager NavigationManager { get; set; } = default!;

        public string IsOpenCssClass => Service.IsOpen ? "is-open" : null;
        protected override void OnInitialized()
        {
            if (Service is null)
                throw new NullReferenceException($"{nameof(ISidepanelService)} has to be registered in the DI container.");
            Service.OnSidepanelChanged += OnSidepanelChanged;
            NavigationManager.LocationChanged += OnLocationChanged;
        }
        public void Dispose()
        {
            Service.OnSidepanelChanged -= OnSidepanelChanged;
            NavigationManager.LocationChanged -= OnLocationChanged;
        }

        public async ValueTask OnSidepanelChanged()
        {
            StateHasChanged();
            if (Service.IsOpen)
            {
                await _element.FocusAsync();
            }
        }

        public void OnKeyDown(KeyboardEventArgs args)
        {
            if (args.Key == "Escape")
                Service.Close();
        }

        /// <summary>
        /// Closes the <see cref="Sidepanel"/> on navigation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLocationChanged(object sender, LocationChangedEventArgs e)
        {
            Service.Close();
        }

        private RenderFragment BuildInnerComponent()
        {
            if (Service.Component is null)
                return null;

            return new RenderFragment(x =>
            {
                x.OpenComponent(0, Service.Component);
                if (Service.Parameters is not null)
                {
                    int sequence = 1;
                    foreach (var parameter in Service.Parameters)
                    {
                        x.AddAttribute(sequence++, parameter.Key, parameter.Value);
                    }
                }
                x.CloseComponent();
            });
        }
    }
}
