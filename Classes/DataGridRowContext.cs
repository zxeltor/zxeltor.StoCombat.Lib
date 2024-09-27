// Copyright (c) 2024, Todd Taylor (https://github.com/zxeltor)
// All rights reserved.
// 
// This source code is licensed under the Apache-2.0-style license found in the
// LICENSE file in the root directory of this source tree.

using zxeltor.Types.Lib.Result;

namespace zxeltor.StoCombat.Lib.Classes;

public class DataGridRowContext
{
    #region Constructors

    public DataGridRowContext(DateTime timestamp, string message, ResultLevel resultLevel = ResultLevel.Info)
    {
        this.Timestamp = timestamp;
        this.ResultLevel = resultLevel;
        this.Message = message;
    }

    public DataGridRowContext(DateTime timestamp, string message, Exception? exception = null, ResultLevel resultLevel = ResultLevel.Error)
    {
        this.Timestamp = timestamp;
        this.ResultLevel = resultLevel;
        this.Message = message;
        this.Exception = exception;
    }

    public DataGridRowContext(DateTime timestamp, Exception? exception = null, ResultLevel resultLevel = ResultLevel.Error)
    {
        this.Timestamp = timestamp;
        this.ResultLevel = resultLevel;
        this.Message = exception.Message;
        this.Exception = exception;
    }

    #endregion

    #region Public Properties

    public DateTime Timestamp { get; set; }
    public Exception? Exception { get; set; }
    public ResultLevel ResultLevel { get; set; }
    public string Message { get; set; }

    #endregion

    #region Public Members

    #region Overrides of Object

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Time={this.Timestamp:HH:mm:ss.fff}, Result={this.ResultLevel}, Message={this.Message}";
    }

    #endregion

    #endregion
}