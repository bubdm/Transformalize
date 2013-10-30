﻿using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;

namespace Transformalize.Operations.Validate
{
    public class NotNullValidatorOperation : ValidationOperation {
        public NotNullValidatorOperation(string keyToValidate, string outKey, string message, bool negated, bool append)
            : base(keyToValidate, outKey, append) {
            Validator = new NotNullValidator(negated, message) { Tag = keyToValidate };
            }
    }
}