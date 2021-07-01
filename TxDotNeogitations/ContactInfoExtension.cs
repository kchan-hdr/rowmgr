using System;
using System.Collections.Generic;
using System.Text;

namespace TxDotNeogitations
{
    public partial class ContactInfo
    {
        public string FullName => string.Join(" ", new[] { this.FirstName, this.LastName } );
        public string PreferredNumber 
        {
            get
            {
                switch( this.PreferredContactMode)
                {

                    default:
                        if ( string.IsNullOrEmpty(this.CellPhone))
                        {
                            if ( string.IsNullOrEmpty(this.HomePhone))
                            {
                                return this.WorkPhone;
                            }
                            else
                            {
                                return this.HomePhone;
                            }
                        } 
                        else
                        {
                            return this.CellPhone;
                        }
                }
            }
        }
    }
}
