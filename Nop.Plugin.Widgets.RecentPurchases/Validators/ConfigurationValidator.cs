using FluentValidation;
using Nop.Plugin.Widgets.RecentPurchases.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Widgets.RecentPurchases.Validators
{
    public partial class ConfigurationValidator : BaseNopValidator<ConfigurationModel>
    {
        public ConfigurationValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.WindowSize)
                .NotEmpty()
                .WithMessage(localizationService.GetResource(Constants.LocaleResources.RecentPurchases_Fields_WindowSize_Required))
                .GreaterThan(0)
                .WithMessage(localizationService.GetResource(Constants.LocaleResources.RecentPurchases_Fields_WindowSize_MustBeGreaterThanZero));
            RuleFor(x => x.WindowTypeId)
                .NotEmpty()
                .WithMessage(localizationService.GetResource(Constants.LocaleResources.RecentPurchases_Fields_WindowType_Required));
            RuleFor(x => x.ZoneId)
                .NotEmpty()
                .WithMessage(localizationService.GetResource(Constants.LocaleResources.RecentPurchases_Fields_Zone_Required));
        }
    }
}
