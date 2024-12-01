using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactManager.Filters;

public class SkipFilter : Attribute, IFilterMetadata
{
}