using System;
using System.Collections.Generic;
using System.Text;

namespace GetTogether.Data
{
    public class ParameterCollection : List<Parameter>
    {
        #region Constructors
        public ParameterCollection()
        {

        }
        public ParameterCollection(params Parameter[] pars)
        {
            foreach (Parameter p in pars) this.Add(p);
        }
        #endregion

        #region Parenthesis

        public void AddStartParenthesis()
        {
            Parameter p = new Parameter();
            p.IsStartParenthesis = true;
            this.Add(p);
        }
        public void AddEndParenthesis()
        {
            Parameter p = new Parameter();
            p.IsEndParenthesis = true;
            this.Add(p);
        }
        public void AddStartParenthesisWithAnd()
        {
            Parameter p = new Parameter();
            p.IsStartParenthesis = true;
            p.ParType = ParameterType.And;
            this.Add(p);
        }
        public void AddStartParenthesisWithOr()
        {
            Parameter p = new Parameter();
            p.IsStartParenthesis = true;
            p.ParType = ParameterType.Or;
            this.Add(p);
        }

        #endregion

        public void AddValue(object column, object value)
        {
            this.Add(new Parameter(column, value));
        }

        public void AddValue(string column, object value)
        {
            this.Add(new Parameter(column, value));
        }

        public void AddCondition(ParameterType parType, TokenTypes tokType, object column, object value)
        {
            this.Add(new Parameter(parType, tokType, column, value));
        }

        public void AddCondition(ParameterType parType, TokenTypes tokType, string column, object value)
        {
            this.Add(new Parameter(parType, tokType, column, value));
        }
    }
}
