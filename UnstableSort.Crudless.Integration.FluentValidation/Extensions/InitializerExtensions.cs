﻿using UnstableSort.Crudless.Integration.FluentValidation;

namespace UnstableSort.Crudless
{
    public static class IncludeFluentValidationInitializer
    {
        public static CrudlessInitializer UseFluentValidation(this CrudlessInitializer initializer)
        {
            return initializer.AddInitializer(new FluentValidationInitializer());
        }
    }
}
