﻿/*******************************************************************************
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
using System.Linq;
using System.Reflection;

namespace Amazon.Util.Internal
{
    public static partial class TypeFactory
    {
        class TypeInfoWrapper : AbstractTypeInfo
        {
            TypeInfo _typeInfo;

            internal TypeInfoWrapper(Type type)
                : base(type)
            {
                this._typeInfo = type.GetTypeInfo();
            }

            public override Type BaseType
            {
                get { return _typeInfo.BaseType; }
            }

            public override Type GetInterface(string name)
            {
                return this._typeInfo.ImplementedInterfaces.FirstOrDefault(x => (x.Namespace + "." + x.Name) == name);
            }

            public override IEnumerable<PropertyInfo> GetProperties()
            {
                return this._type.GetProperties();
            }

            public override IEnumerable<FieldInfo> GetFields()
            {
                return this._type.GetFields();
            }

            public override bool IsClass
            {
                get { return this._typeInfo.IsClass; }
            }

            public override bool IsEnum
            {
                get { return this._typeInfo.IsEnum; }
            }

            public override MethodInfo GetMethod(string name, ITypeInfo[] paramTypes)
            {
                Type[] types = new Type[paramTypes.Length];
                for (int i = 0; i < paramTypes.Length; i++)
                    types[i] = ((AbstractTypeInfo)paramTypes[i]).Type;

                return this._type.GetMethod(name, types);
            }

            public override PropertyInfo GetProperty(string name)
            {
                return this._type.GetProperty(name);
            }

            public override bool IsAssignableFrom(ITypeInfo typeInfo)
            {
                return this._typeInfo.IsAssignableFrom(((TypeInfoWrapper)typeInfo)._typeInfo);
            }
        }
    }
}
