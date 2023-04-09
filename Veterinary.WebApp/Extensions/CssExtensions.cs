using System.Linq;
using Microsoft.AspNetCore.Components.Forms;

namespace Veterinary.WebApp.Extensions;

public class CssExtensions: FieldCssClassProvider
{
    public override string GetFieldCssClass(EditContext editContext, in FieldIdentifier fieldIdentifier)
    {
        var isValid = !editContext.GetValidationMessages(fieldIdentifier).Any();

        if (editContext.IsModified(fieldIdentifier))
        {
            return isValid ? "custom-succes" : "custom-error";
        }

        return isValid ? "" : "custom-error";
    }
}
