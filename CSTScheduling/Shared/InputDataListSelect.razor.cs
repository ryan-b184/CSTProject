using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using System.Linq.Expressions;
using System.Linq;
using System;
using Microsoft.AspNetCore.Components.Web;

/// <summary> 
/// PURPOSE:
/// This component is necessary for our project because Blazor <EditForm> does not natively support the HTML5 datalist.
///
/// Essentially, this component wraps the basic functionality of an HTML5 <input> with <datalist>
/// See more: https://www.w3schools.com/tags/tag_datalist.asp?msclkid=6b626864a55e11ecab7a6051a95adf16)
/// Supported browsers: https://caniuse.com/?search=datalist
///
/// - Jonathan Cruz (CST224)
/// - John Sierra (CST213)
/// 
///     =======================================>
///     This custom Blazor component originally developed by Shaun C Curtis (23rd April, 2021: Initial version): 
///     https://www.codeproject.com/Articles/5300708/Building-a-DataList-Control-in-Blazor
/// 
///     The parent classes are written by Microsoft, and can be found here:
///     https://github.com/dotnet/aspnetcore/blob/main/src/Components/Web/src/Forms/InputBase.cs
/// 
///     The component's source code is licensed under the Code Project Open License (CPOL):
///     https://www.codeproject.com/info/cpol10.aspx
///     =======================================>
/// </summary>
namespace CSTScheduling.Shared
{
    public partial class InputDataListSelect<TValue> : InputBase<TValue>
    {
        [Parameter]
        public string DataListID { get; set; }

        [DisallowNull] public ElementReference? Element { get; protected set; }

        // the EditContext ValidationMessageStore
        private ValidationMessageStore? _parsingValidationMessages;

        // field to manage parsing failure
        private bool _previousParsingAttemptFailed = false;

        // List of values for datalist
        [Parameter] public Dictionary<TValue, string> DataList { get; set; }

        // parameter to restrict valid values to the list
        [Parameter] public bool RestrictToList { get; set; }

        // unique id for the datalist based on a guid - we may have more than one in a form
       // private string dataListId { get; set; } = Guid.NewGuid().ToString();

        // instruction to CurrentStringValue that we are in RestrictToList mode 
        // and the user has tabbed
        private bool _setValueByTab = false;

        // current typed value in the input box - kept up to date by UpdateEnteredText
        private string _typedText = string.Empty;

        // New method to parallel CurrentValueAsString
        protected string CurrentStringValue
        {
            get
            {
                // check if we have a match to the datalist and get the value from the K/V pair
                if (DataList != null && DataList.Any(item => item.Key.Equals(this.Value)))
                    return DataList.First(item => item.Key.Equals(this.Value)).Value;
                // if not return an empty string
                return string.Empty;
            }
            set
            {
                // Check if we have a ValidationMessageStore
                // Either get one or clear the existing one
                if (_parsingValidationMessages == null)
                    _parsingValidationMessages = new ValidationMessageStore(EditContext);
                else
                    _parsingValidationMessages?.Clear(FieldIdentifier);

                // Set defaults
                TValue val = default;
                var _havevalue = false;
                // check if we have a previous valid value - we'll stick with 
                // this is the current attempt to set the value is invalid
                //var _havepreviousvalue = DataList != null && DataList.ContainsKey(this.Value);
                var _havepreviousvalue = true;

                // Set the value by tabbing.   We need to select the first entry in the DataList
                if (_setValueByTab)
                {
                    if (!string.IsNullOrWhiteSpace(this._typedText))
                    {
                        // Check if we have at least one K/V match in the filtered list
                        _havevalue = DataList != null && DataList.Any
                        (item => item.Value.Contains(_typedText,
                        StringComparison.CurrentCultureIgnoreCase));
                        if (_havevalue)
                        {
                            // the the first K/V pair
                            var filteredList = DataList.Where(item => item.Value.Contains
                            (_typedText, StringComparison.CurrentCultureIgnoreCase)).ToList();
                            val = filteredList[0].Key;
                        }
                    }
                }
                // Normal set
                else
                {
                    // Check if we have a match and set it if we do
                    _havevalue = DataList != null && DataList.ContainsValue(value);
                    if (_havevalue)
                        val = DataList.First(item => item.Value.Equals(value)).Key;
                }

                // Check if the inputbox is empty
                if (string.IsNullOrWhiteSpace(this._typedText))
                {
                    //set input box to default
                    this.CurrentValue = default;
                }

                 // check if we have a valid value
                if (_havevalue)
                {
                    // assign it to current value - this will kick off 
                    // a ValueChanged notification on the EditContext
                    this.CurrentValue = val;
                    // Check if the last entry failed validation.
                    // If so notify the EditContext that validation has changed, i.e., it's now clear
                    if (_previousParsingAttemptFailed)
                    {
                        EditContext.NotifyValidationStateChanged();
                        _previousParsingAttemptFailed = false;
                    }
                }
                // We don't have a valid value
                else
                {
                    // check if we're reverting to the last entry.
                    // If we don't have one the generate error message
                    if (!_havepreviousvalue)
                    {
                        // No K/V match so add a message to the message store
                        _parsingValidationMessages?.Add(FieldIdentifier,
                                "You must choose a valid selection");
                        // keep track of validation state for the next iteration
                        _previousParsingAttemptFailed = true;
                        // notify the EditContext whick will precipitate 
                        // a Validation Message general update
                        EditContext.NotifyValidationStateChanged();
                    }
                }
                // Clear the Tab notification flag
                _setValueByTab = false;
            }
        }

        // Keep _typedText up to date with typed entry
        //private void UpdateEnteredText(ChangeEventArgs e) => _typedText = e.Value?.ToString();
        private void UpdateEnteredText(ChangeEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.Value?.ToString()))
            {
                _typedText = "";
            }
            else
            {
                _typedText = e.Value?.ToString();
            }
        }


        // Detector for Tabbing away from the input
        private void OnKeyDown(KeyboardEventArgs e)
        {
            // Check if we have a Tab with some text already typed
            _setValueByTab = ((!string.IsNullOrWhiteSpace(e.Key)) &&
            e.Key.Equals("Tab") && !string.IsNullOrWhiteSpace(this._typedText));
        }

        // set as blind
        protected override bool TryParseValueFromString(string? value, [MaybeNullWhen(false)] out TValue result, [NotNullWhen(false)] out string validationErrorMessage)
        {
            throw new NotSupportedException($"This component does not parse normal string inputs. Bind to the '{nameof(CurrentValue)}' property, not '{nameof(CurrentValueAsString)}'.");
        }
    }
}

