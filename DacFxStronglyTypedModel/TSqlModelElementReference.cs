﻿//------------------------------------------------------------------------------
//<copyright company="Microsoft">
//
//    The MIT License (MIT)
//    
//    Copyright (c) 2015 Microsoft
//    
//    Permission is hereby granted, free of charge, to any person obtaining a copy
//    of this software and associated documentation files (the "Software"), to deal
//    in the Software without restriction, including without limitation the rights
//    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//    copies of the Software, and to permit persons to whom the Software is
//    furnished to do so, subject to the following conditions:
//    
//    The above copyright notice and this permission notice shall be included in all
//    copies or substantial portions of the Software.
//    
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//    SOFTWARE.
//</copyright>
//------------------------------------------------------------------------------

using Microsoft.SqlServer.Dac.Model;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.SqlServer.Dac.Extensions.Prototype
{
    public class TSqlModelElementReference : TSqlModelElement, ISqlModelElementReference
    {
        private ModelRelationshipInstance relationshipInstance;
        private ModelTypeClass predefinedTypeClass;


        public TSqlModelElementReference(ModelRelationshipInstance relationshipReference) :
            base()
        {
            relationshipInstance = relationshipReference;
        }
        public TSqlModelElementReference(ModelRelationshipInstance relationshipReference, ModelTypeClass typeClass) :
            this(relationshipReference)
        {
            
            if (relationshipInstance.Object != null && relationshipInstance.Object.ObjectType != typeClass)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                ModelMessages.InvalidObjecType, relationshipInstance.Object.ObjectType.Name, typeClass.Name),
                "typeClass");
            }

            predefinedTypeClass = typeClass;
        }

        public override ObjectIdentifier Name
        {
            get
            {
                return relationshipInstance.ObjectName;
            }
        }

        public override ModelTypeClass ObjectType
        {
            get
            {
                if (IsResovled())
                {
                    return base.ObjectType;
                }
                else
                {
                    // when object is unresolved default to the predefined ModelTypClass
                    return predefinedTypeClass;
                }
            }
        }
        public bool IsResovled()
        {
            return relationshipInstance.Object != null;
        }

        public override TSqlObject Element
        {
            get
            {
                // Verify the Element is resolved.
                if (!IsResovled())
                {
                    throw new UnresolvedElementException(
                       string.Format(CultureInfo.CurrentUICulture,
                       ModelMessages.UnresolvedObject,
                       relationshipInstance.ObjectName));
                }
                return relationshipInstance.Object;
            }

        }

        protected T GetMetdataProperty<T>(ModelPropertyClass property)
        {
            return relationshipInstance.GetProperty<T>(property);
        }
    }
}