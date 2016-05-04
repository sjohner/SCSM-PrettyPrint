/* 
 * Copyright (C) 2014 - 2016 Stefan Johner <sjohner@posteo.ch>
 * This file is part of Service Manager PrettyPrint.
 * 
 * Service Manager PrettyPrint is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * Service Manager PrettyPrint is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//Contains ValidationRule
using System.Windows.Controls;

//Contains CultureInfo
using System.Globalization;

namespace scsmlab.prettyprint.Classes
{
    public class TextBoxEmptyRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                if (string.IsNullOrEmpty((string)value))
                {
                    return new ValidationResult(false, "Please enter text.");
                }
                return new ValidationResult(true, null);
            }
            catch
            {
                return new ValidationResult(false, null);
            }
        }
    }
}
