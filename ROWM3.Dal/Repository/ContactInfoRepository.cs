using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROWM.Dal
{
    public class ContactInfoRepository
    {
        readonly ROWM_Context _context;

        public ContactInfoRepository(ROWM_Context c) => _context = c;

        public async Task<Organization> FindOrganization(string n) =>
            await _context.Organization.SingleOrDefaultAsync(ox => ox.Name.Equals(n));

        public async Task<Organization> Get(Guid id) =>
            await _context.Organization.FindAsync(id);
    }
}
