/*******************************************************************************
 *  Copyright Amazon.com, Inc. or its affiliates. All Rights Reserved.
 *  Licensed under the Apache License, Version 2.0 (the "License"). You may not use
 *  this file except in compliance with the License. A copy of the License is located at
 *
 *  http://aws.amazon.com/apache2.0
 *
 *  or in the "license" file accompanying this file.
 *  This file is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
 *  CONDITIONS OF ANY KIND, either express or implied. See the License for the
 *  specific language governing permissions and limitations under the License.
 * *****************************************************************************
 *    __  _    _  ___
 *   (  )( \/\/ )/ __)
 *   /__\ \    / \__ \
 *  (_)(_) \/\/  (___/
 *
 *  AWS SDK for .NET
 *
 */
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Amazon.Util.Internal
{
    public interface ITypeInfo
    {
        Type BaseType { get; }

        Type Type { get; }

        Type GetInterface(string name);

        IEnumerable<PropertyInfo> GetProperties();

        IEnumerable<FieldInfo> GetFields();

        MethodInfo GetMethod(string name, ITypeInfo[] paramTypes);

        ConstructorInfo GetConstructor(ITypeInfo[] paramTypes);

        PropertyInfo GetProperty(string name);

        bool IsAssignableFrom(ITypeInfo typeInfo);

        bool IsEnum {get;}

        bool IsClass { get; }
    }

    public static partial class TypeFactory
    {
        public static readonly ITypeInfo[] EmptyTypes = new ITypeInfo[] { };
        public static ITypeInfo GetTypeInfo(Type type)
        {
            if (type == null)
                return null;

            return new TypeInfoWrapper(type);
        }

        abstract class AbstractTypeInfo : ITypeInfo
        {
            protected Type _type;

            internal AbstractTypeInfo(Type type)
            {
                this._type = type;
            }

            public Type Type
            {
                get{return this._type;}
            }

            public override int GetHashCode()
            {
                return this._type.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                var typeWrapper = obj as AbstractTypeInfo;
                if (typeWrapper == null)
                    return false;

                return this._type.Equals(typeWrapper._type);
            }

            public abstract Type BaseType { get; }
            public abstract Type GetInterface(string name);
            public abstract IEnumerable<PropertyInfo> GetProperties();
            public abstract IEnumerable<FieldInfo> GetFields();
            public abstract MethodInfo GetMethod(string name, ITypeInfo[] paramTypes);
            public abstract PropertyInfo GetProperty(string name);
            public abstract bool IsAssignableFrom(ITypeInfo typeInfo);
            public abstract bool IsClass { get; }
            public abstract bool IsEnum { get; }
            public abstract ConstructorInfo GetConstructor(ITypeInfo[] paramTypes);
       }
    }


}
