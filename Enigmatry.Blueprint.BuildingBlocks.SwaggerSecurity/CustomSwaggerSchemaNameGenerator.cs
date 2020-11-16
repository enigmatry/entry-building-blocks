using NJsonSchema.Generation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Enigmatry.Blueprint.BuildingBlocks.Swagger
{
    internal class CustomSwaggerSchemaNameGenerator : DefaultSchemaNameGenerator
    {
        public override string Generate(Type type)
        {
            // fixes the problem when the type is declared within another type
            // e.g. GetUsers.Model ()
            // inspired by https://stackoverflow.com/questions/45241177/nswag-namespace-in-model-names
            var declaringTypes = new List<Type>();
            GetAllDeclaringTypes(declaringTypes, type);

            return declaringTypes.Count > 0
                ? $"{String.Join("", declaringTypes.Select(t => t.Name))}{type.Name}"
                : base.Generate(type);
        }

        private static void GetAllDeclaringTypes(IList<Type> declaringTypes, Type type)
        {
            while (true)
            {
                if (type.DeclaringType != null)
                {
                    declaringTypes.Insert(0, type.DeclaringType);
                    type = type.DeclaringType;
                    continue;
                }

                break;
            }
        }
    }
}
