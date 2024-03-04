using Makers.Database.Entities;
using Makers.Security;
using Makers.Utilities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Data;

namespace Makers.Controllers;

public partial class DashboardController
{
    [HttpPost]
    public IActionResult GetRole([FromBody] JObject reqBody)
    {
        var Id = reqBody.GetParameter<int>("Id");

        var roleClaims = (from r in db.T_MAP_ROLES_CLAIMS
                          where r.ROLE_ID == Id
                          select r.CLAIM_VALUE).ToList();

        var data = (from r in db.T_ROLES
                    where r.ID == Id && r.ID != Constants.GlobalAdminId
                    select new { r.ID, r.ROLE_NAME, CLAIM_VALUE = roleClaims }).ToList();

        return this.Response(null, data);
    }

    [HttpPost]
    public IActionResult GetRoles([FromBody] JObject reqBody)
    {
        //var PageNumber = reqBody.GetParameter<int>("PageNumber");
        //var PageSize = reqBody.GetParameter<int>("PageSize");
        //var Filter = reqBody.Value<string>("Filter");

        var data = from r in db.T_ROLES
                   where r.ID != Constants.GlobalAdminId
                   orderby r.ID descending
                   select r;

        //if (!string.IsNullOrEmpty(Filter))
        //{
        //    data = data.Where(e => e.ROLE_NAME.Contains(Filter)).OrderByDescending(e => e.ID);
        //}

        //var dataCount = data.Count();

        //var resultData = SecurityHelper.Paging(PageSize, dataCount, data.Skip((PageNumber - 1) * PageSize).Take(PageSize));

        return this.Response(null, data);
    }

    [HttpPost]
    public async Task<IActionResult> AddRole([FromBody] JObject reqBody)
    {
        var RoleName = reqBody.GetParameter<string>("ROLE_NAME");
        var RoleClaims = reqBody.GetValue("CLAIM_VALUE");

        if (RoleName == Constants.GlobalAdmin)
        {
            throw new Exception(Constants.ResponseUnauthorized);
        }

        T_ROLES newRole = new()
        {
            ID = null,
            INSDATE = DateTime.Now,
            LUPDATE = DateTime.Now,
            ROLE_NAME = RoleName,
            IS_ACTIVE = Constants.Yes
        };

        await db.T_ROLES.AddAsync(newRole);

        await db.SaveChangesAsync();

        await db.AuditAsync(jwt, Constants.AuditActionInsert, newRole, $"Role Name: {RoleName}");

        foreach (var claim in RoleClaims)
        {
            T_MAP_ROLES_CLAIMS roleClaims = new()
            {
                ID = null,
                INSDATE = DateTime.Now,
                LUPDATE = DateTime.Now,
                ROLE_ID = newRole.ID,
                CLAIM_VALUE = claim.ToString()
            };

            await db.T_MAP_ROLES_CLAIMS.AddAsync(roleClaims);
        }

        await db.SaveChangesAsync();

        await db.AuditAsync(jwt, Constants.AuditActionInsert, newRole, $"Role Name: {RoleName}", true);

        return this.Response("Role added successfully", null);
    }

    [HttpPost]
    public async Task<IActionResult> EditRole([FromBody] JObject reqBody)
    {
        var EditEntityId = reqBody.GetParameter<int>("EditEntityId");
        var RoleName = reqBody.GetParameter<string>("ROLE_NAME");
        var RoleClaims = reqBody.GetValue("CLAIM_VALUE");

        if (EditEntityId == Constants.GlobalAdminId)
        {
            throw new Exception(Constants.ResponseUnauthorized);
        }

        if (jwt.RoleId == EditEntityId)
        {
            throw new Exception($"{Constants.ResponseUnauthorized}: You are not allowed to edit your role");
        }

        var role = db.T_ROLES.First(e => e.ID == EditEntityId);
        var roleClaims = db.T_MAP_ROLES_CLAIMS.Where(e => e.ROLE_ID == EditEntityId).ToList();

        role.ROLE_NAME = RoleName;
        role.LUPDATE = DateTime.Now;

        db.T_ROLES.Update(role);

        await db.AuditAsync(jwt, Constants.AuditActionUpdate, role, $"Role Name: {RoleName}");

        db.T_MAP_ROLES_CLAIMS.RemoveRange(roleClaims);

        await db.SaveChangesAsync();

        foreach (var claim in RoleClaims)
        {
            T_MAP_ROLES_CLAIMS newRoleClaims = new()
            {
                ID = null,
                INSDATE = DateTime.Now,
                LUPDATE = DateTime.Now,
                ROLE_ID = EditEntityId,
                CLAIM_VALUE = claim.ToString()
            };

            await db.T_MAP_ROLES_CLAIMS.AddAsync(newRoleClaims);
        }

        await db.SaveChangesAsync();

        await db.AuditAsync(jwt, Constants.AuditActionUpdate, role, $"Role Name: {RoleName}", true);

        return this.Response("Role updated successfully", null);
    }

    [HttpPost]
    public async Task<IActionResult> ChangeRoleStatus([FromBody] JObject reqBody)
    {
        var Id = reqBody.GetParameter<int>("Id");

        if (Id == Constants.GlobalAdminId)
        {
            throw new Exception(Constants.ResponseUnauthorized);
        }

        var role = db.T_ROLES.First(e => e.ID == Id);

        if (role.IS_ACTIVE == Constants.Yes)
        {
            role.IS_ACTIVE = Constants.No;
        }
        else
        {
            role.IS_ACTIVE = Constants.Yes;
        }

        db.T_ROLES.Update(role);

        await db.AuditAsync(jwt, Constants.AuditActionUpdate, role, $"Role Name: {role.ROLE_NAME}");

        await db.SaveChangesAsync();

        return this.Response("Role changed status successfully", null);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteRole([FromBody] JObject reqBody)
    {
        var Id = reqBody.GetParameter<int>("Id");

        var Role = db.T_ROLES.First(e => e.ID == Id);

        db.T_ROLES.Remove(Role);

        await db.AuditAsync(jwt, Constants.AuditActionDelete, Role, $"Role Name: {Role.ROLE_NAME}");

        await db.SaveChangesAsync();

        return this.Response("Role deleted successfully", null);
    }
}

