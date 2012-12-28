using System;
using System.Collections.Generic;
using System.Text;

namespace GetTogether.Data
{
    public class Parameter
    {
        #region Attributes
        private ParameterType _ParType = ParameterType.Initial;
        private TokenTypes _TokenType;
        private string _Column;
        private object _Value;
        private string _ColumnPar;
        private bool _IsStartParenthesis;

        public bool IsStartParenthesis
        {
            get { return _IsStartParenthesis; }
            set { _IsStartParenthesis = value; }
        }
        private bool _IsEndParenthesis;

        public bool IsEndParenthesis
        {
            get { return _IsEndParenthesis; }
            set { _IsEndParenthesis = value; }
        }

        public ParameterType ParType
        {
            get { return _ParType; }
            set { _ParType = value; }
        }
        public TokenTypes TokType
        {
            get { return _TokenType; }
            set { _TokenType = value; }
        }
        public string Column
        {
            get { return _Column; }
            set { _Column = value; _ColumnPar = value; }
        }
        public object Value
        {
            get { return _Value; }
            set { _Value = value; }
        }
        public string ColumnPar
        {
            get { return _ColumnPar; }
            set { _ColumnPar = value; }
        }
        #endregion

        #region Constructors
        public Parameter(ParameterType parType, TokenTypes tokType, string column, object value)
        {
            this.TokType = tokType;
            this.ParType = parType;
            this.Column = column;
            this.Value = value;
        }
        public Parameter(ParameterType parType, TokenTypes tokType, object column, object value)
        {
            this.TokType = tokType;
            this.ParType = parType;
            this.Column = column.ToString();
            this.Value = value;
        }
        public Parameter(TokenTypes tokType, object column, object value)
        {
            this.TokType = tokType;
            this.ParType = ParameterType.Initial;
            this.Column = column.ToString();
            this.Value = value;
        }
        public Parameter(object column, object value)
        {
            this.Column = column.ToString();
            this.Value = value;
        }

        public Parameter()
        {

        }
        #endregion
    }
}
 