using Makers.Utilities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Makers.Controllers;

public partial class DashboardController
{
    [HttpPost]
    public IActionResult ClaimsDropdown([FromBody] JObject reqBody)
    {
        var data = from c in db.T_CLAIMS
                   orderby c.GROUP_ID, c.ID
                   select new
                   {
                       label = c.CLAIM_NAME,
                       value = c.CLAIM_VALUE
                   };

        return this.Response(null, data);
    }

    [HttpPost]
    public IActionResult ProjectDropdown([FromBody] JObject reqBody)
    {
        var data = from c in db.T_PROJECTS
                   where c.IS_ACTIVE == Constants.Yes
                   orderby c.ID
                   select new
                   {
                       label = c.NAMEX,
                       value = c.ID
                   };

        return this.Response(null, data);
    }

    [HttpPost]
    public IActionResult InstDropdown([FromBody] JObject reqBody)
    {
        var data = from c in db.T_INST
                   where c.IS_ACTIVE == Constants.Yes
                   orderby c.ID
                   select new
                   {
                       label = c.NAMEX,
                       value = c.ID
                   };

        return this.Response(null, data);
    }

    [HttpPost]
    public IActionResult TrainersDropdown([FromBody] JObject reqBody)
    {
        var data = from c in db.T_TRAINERS
                   where c.IS_ACTIVE == Constants.Yes
                   orderby c.ID
                   select new
                   {
                       label = c.NAMEX,
                       value = c.ID
                   };

        return this.Response(null, data);
    }
    [HttpPost]
    public IActionResult TrainingDropdown([FromBody] JObject reqBody)
    {
        var data = from c in db.T_TRAINING
                   where c.IS_ACTIVE == Constants.Yes
                   orderby c.ID
                   select new
                   {
                       label = c.NAMEX,
                       value = c.ID
                   };

        return this.Response(null, data);
    }

    [HttpPost]
    public IActionResult DictDropdown([FromBody] JObject reqBody)
    {
        var Key = reqBody.GetParameter<string>("Key");

        var data = from u in db.T_DICT
                   where u.KEYX == Key
                   select new
                   {
                       label = u.VALUEX,
                       value = u.VALUEX
                   };

        return this.Response(null, data);
    }

    [HttpPost]
    public IActionResult GetRolesDropdown([FromBody] JObject reqBody)
    {
        List<dynamic> data = null;

        if (jwt.RoleId == Constants.GlobalAdminId)
        {
            data = (from r in db.T_ROLES
                    where r.IS_ACTIVE == Constants.Yes
                    select new { label = r.ROLE_NAME, value = r.ID }).ToList<dynamic>();
        }

        else
        {
            data = (from r in db.T_ROLES
                    where r.ID != Constants.GlobalAdminId && r.IS_ACTIVE == Constants.Yes
                    select new { label = r.ROLE_NAME, value = r.ID }).ToList<dynamic>();
        }

        return this.Response(null, data);
    }

    [HttpPost]
    public IActionResult ProjectsDropdown([FromBody] JObject reqBody)
    {
        var data = from c in db.T_PROJECTS
                   where c.IS_ACTIVE == Constants.Yes
                   orderby c.ID
                   select new
                   {
                       label = c.NAMEX,
                       value = c.ID
                   };

        return this.Response(null, data);
    }
}


