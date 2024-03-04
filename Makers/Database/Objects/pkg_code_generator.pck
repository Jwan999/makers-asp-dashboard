CREATE OR REPLACE PACKAGE pkg_code_generator IS
  const_owner CONSTANT VARCHAR2(20) := 'TEST';
  const_app   CONSTANT VARCHAR2(16) := 'Makers';
  n           CONSTANT VARCHAR2(1) := chr(10); -- New line.
  t           CONSTANT VARCHAR2(1) := chr(9); -- Tab.

  PROCEDURE common_controller(p_table_name  VARCHAR2,
                              p_screen_name VARCHAR2);

  PROCEDURE entity(p_table_name VARCHAR2);

  FUNCTION conv_type(p_data_type      IN VARCHAR2,
                     p_data_precision NUMBER DEFAULT NULL,
                     p_data_scale     NUMBER DEFAULT NULL,
                     p_column_name    VARCHAR2 DEFAULT NULL) RETURN VARCHAR2;

  PROCEDURE routes(p_screen_name VARCHAR2);

/*  PROCEDURE postman(p_table_name VARCHAR2);
*/
END pkg_code_generator;
/
CREATE OR REPLACE PACKAGE BODY pkg_code_generator IS
  PROCEDURE common_controller(p_table_name  VARCHAR2,
                              p_screen_name VARCHAR2) IS
    v_output    CLOB;
    v_params    CLOB;
    v_upd_obj   CLOB;
    v_added_obj CLOB;
    v_vname     CLOB;
    v_type      CLOB;
    v_inst_auth CLOB;
    v_get_items CLOB;
  
    v_screen_name VARCHAR2(25) := REPLACE(initcap(p_screen_name), ' ', '');
    b             VARCHAR2(100) := chr(9) || chr(9) || chr(9) || chr(9) ||
                                   chr(9) || chr(9); --body space
  BEGIN
    FOR rec IN (SELECT *
                  FROM all_tab_columns
                 WHERE owner = const_owner
                   AND table_name = p_table_name
                 ORDER BY column_id)
    
    LOOP
      IF rec.column_name NOT IN ('ID',
                                 'INSDATE',
                                 'INSERT_DATE',
                                 'LUPDATE',
                                 'LAST_UPDATE',
                                 'IS_ACTIVE')
      THEN
        v_type := conv_type(p_data_type      => rec.data_type,
                            p_data_precision => rec.data_precision,
                            p_data_scale     => rec.data_scale,
                            p_column_name    => rec.column_name);
      
        v_vname := REPLACE(initcap(rec.column_name), '_', '');
      
        v_params    := v_params || b || 'var ' || v_vname ||
                       ' = reqBody.GetParameter<' ||
                       REPLACE(v_type, '?', '') || '>("' || rec.column_name ||
                       '");' || n;
        v_added_obj := v_added_obj || b || chr(9) || t || rec.column_name ||
                       ' = ' || v_vname || ',' || n;
      
        v_upd_obj := v_upd_obj || b || v_screen_name || '.' ||
                     rec.column_name || ' = ' || v_vname || ';' || n;
      
        v_get_items := v_get_items || n || b || b || 'e.' ||
                       rec.column_name || ',';
      END IF;
    
      IF v_inst_auth IS NULL
         AND rec.column_name LIKE '%INST%'
      THEN
        v_inst_auth := 'jwt.UserInstitutions.Contains(e.' ||
                       rec.column_name || ')';
      END IF;
    END LOOP;
  
    v_output := REPLACE('using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using %app.Database.Entities;
using %app.Security;
using %app.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace %app.Controllers
{
    public partial class DataController
    {
',
                        '%app',
                        const_app) ||
                REPLACE(REPLACE('        [HttpPost]
        public IActionResult Get' ||
                                REPLACE(initcap(pkg_utils.pluralize(p_screen_name)),
                                        ' ',
                                        '') || '([FromBody] JObject reqBody)
        {
            var PageNumber = reqBody.GetParameter<int>("PageNumber");
            var PageSize = reqBody.GetParameter<int>("PageSize");
            var Filter = reqBody.GetParameter<string>("Filter");
            
            var data = from e in db.%p_tname
                       orderby e.ID descending
                       ' || CASE
                                  WHEN v_inst_auth IS NOT NULL THEN
                                   'where ' || v_inst_auth || n || '                       '
                                END || 'select new {
                        e.ID,
                        e.IS_ACTIVE,' ||
                                v_get_items || '
                        };

            if (!string.IsNullOrEmpty(Filter))
            {
                data = data.Where(e => e.SOMETHING.Contains(Filter)).OrderByDescending(e => e.ID);
            }

            var dataCount = data.Count();

            var resultData = SecurityHelper.Paging(PageSize, dataCount, data.Skip((PageNumber - 1) * PageSize).Take(PageSize));

            return this.Response(null, resultData);
        }

        [HttpPost]
        public IActionResult Get%v_sname([FromBody] JObject reqBody)
        {
            var Id = reqBody.GetParameter<int>("Id");

            var data = from e in db.%p_tname
                       where e.ID == Id ' || CASE
                                  WHEN v_inst_auth IS NOT NULL THEN
                                   '&& ' || v_inst_auth || '                        '
                                END || '
                       select e;

            return this.Response(null, data);
        }

        [HttpPost]
        public async Task<IActionResult> Add%v_sname([FromBody] JObject reqBody)
        {
' || v_params || '
            %p_tname new%v_sname = new()
            {
                ID = null,
                INSDATE = DateTime.Now,
                LUPDATE = DateTime.Now,
' || v_added_obj || '            };

           await db.%p_tname.AddAsync(new%v_sname);

           await db.SaveChangesAsync();

           await db.AuditAsync(jwt, Constants.AuditActionInsert, new%v_sname, $"%v_sname SOMETHING: {%v_sname.SOMETHING}", true);

           return this.Response("%v_sname added successfully", null);
        }

        [HttpPost]
        public async Task<IActionResult> Edit%v_sname([FromBody] JObject reqBody)
        {
            var EditEntityId = reqBody.GetParameter<int>("EditEntityId");
' || v_params || '
            var %v_sname = db.%p_tname.First(e => e.ID == EditEntityId' || CASE
                                  WHEN v_inst_auth IS NOT NULL THEN
                                   ' && ' || v_inst_auth
                                END || ');

' || v_upd_obj ||
                                '            %v_sname.LUPDATE = DateTime.Now;

            db.%p_tname.Update(%v_sname);

            await db.AuditAsync(jwt, Constants.AuditActionUpdate, %v_sname, $"%v_sname SOMETHING: {%v_sname.SOMETHING}");

            await db.SaveChangesAsync();

            return this.Response("%v_sname updated successfully", null);
        }

        [HttpPost]
        public async Task<IActionResult> Delete%v_sname([FromBody] JObject reqBody)
        {
            var Id = reqBody.GetParameter<int>("Id");

            var %v_sname = db.%p_tname.First(e => e.ID == Id' || CASE
                                  WHEN v_inst_auth IS NOT NULL THEN
                                   ' && ' || v_inst_auth
                                END || ');

            db.%p_tname.Remove(%v_sname);

            await db.AuditAsync(jwt, Constants.AuditActionDelete, %v_sname, $"%v_sname SOMETHING: {%v_sname.SOMETHING}");

            await db.SaveChangesAsync();

            return this.Response("%v_sname deleted successfully", null);
        }
        
        [HttpPost]
        public async Task<IActionResult> Change%v_snameStatus([FromBody] JObject reqBody)
        {
            var Id = reqBody.GetParameter<int>("Id");

            var %v_sname = db.%p_tname.First(e => e.ID == Id' || CASE
                                  WHEN v_inst_auth IS NOT NULL THEN
                                   ' && ' || v_inst_auth
                                END || ');
                                
            if (%v_sname.IS_ACTIVE == Constants.Yes)
            {
                %v_sname.IS_ACTIVE = Constants.No;
            }
            else
            {
                %v_sname.IS_ACTIVE = Constants.Yes;
            }
            
            db.%p_tname.Update(%v_sname);

            await db.AuditAsync(jwt, Constants.AuditActionUpdate, %v_sname, $"%v_sname SOMETHING: {%v_sname.SOMETHING}");

            await db.SaveChangesAsync();

            return this.Response("%v_sname changes status successfully", null);
        }
    }
}
',
                                '%p_tname',
                                p_table_name),
                        '%v_sname',
                        v_screen_name);
  
    dbms_output.put_line(n || v_screen_name || 'Data.cs' || n);
    dbms_output.put_line(v_output);
  END;

  PROCEDURE entity(p_table_name VARCHAR2) IS
  
    v_output  CLOB; -- Final result.
    v_header  CLOB;
    v_body    CLOB;
    v_footer  VARCHAR2(20) := t || '}' || n || '}';
    v_get_set VARCHAR2(20) := '{ get; set; }';
  
    v_using_system VARCHAR2(20);
    v_data_type    VARCHAR2(20);
  
  BEGIN
    -- Get columns.
    FOR rec IN (SELECT *
                  FROM all_tab_columns
                 WHERE owner = const_owner
                   AND table_name = upper(p_table_name)
                 ORDER BY column_id)
    
    LOOP
    
      IF rec.data_precision >= 19
      THEN
        raise_application_error(-20001,
                                'CODE GENERATOR SPECIFIC ERROR! Numbers with precision beyond 19 are not supported.
                              Please consider changing the datatype of column ' ||
                                rec.column_name || '.');
      END IF;
    
      -- Data type.
      v_data_type := conv_type(p_data_type      => rec.data_type,
                               p_data_precision => rec.data_precision,
                               p_data_scale     => rec.data_scale,
                               p_column_name    => rec.column_name);
    
      -- At least one column is a DATE.
      IF v_data_type = 'DateTime?'
         OR v_data_type = 'Int64?'
      THEN
        v_using_system := 'using System;' || n || n;
      END IF;
    
      -- Construct body.
      v_body := v_body || t || t || 'public ' || v_data_type || ' ' ||
                rec.column_name || ' ' || v_get_set || n;
    
      -- Reset variables.
      v_data_type := NULL;
    
    END LOOP;
  
    -- Construct header.
    v_header := v_using_system || 'namespace ' || const_app ||
                '.Database.Entities' || n || '{' || n || t ||
                'public class ' || p_table_name || n || t || '{' || n;
  
    -- Concatanate header, body and footer.
    v_output := v_output || v_header || v_body || v_footer;
  
    -- Write the result to a file.
    dbms_output.put_line(p_table_name || '.cs' || n);
    dbms_output.put_line(v_output);
  
    dbms_output.put_line(n || 'Db.cs' || n || n || 'public DbSet<' ||
                         p_table_name || '> ' || p_table_name ||
                         ' { get; set; }' || n);
  
  END;

  FUNCTION conv_type(p_data_type      IN VARCHAR2,
                     p_data_precision NUMBER DEFAULT NULL,
                     p_data_scale     NUMBER DEFAULT NULL,
                     p_column_name    VARCHAR2 DEFAULT NULL) RETURN VARCHAR2 IS
  BEGIN
    IF p_data_type = 'VARCHAR2'
       OR p_data_type = 'NVARCHAR2'
       OR p_data_type = 'CLOB'
       OR p_data_type = 'NCLOB'
       OR p_data_type = 'CHAR'
       OR p_data_type = 'NCHAR'
    THEN
      RETURN 'string';
    
    ELSIF p_data_type = 'NUMBER'
          OR p_data_type = 'FLOAT'
          OR p_data_type = 'LONG'
    THEN
      -- C# "double".
      IF (p_data_precision IS NOT NULL AND p_data_scale <> 0) -- NUMBER(10,2)
         OR p_column_name LIKE '%AMOUNT%'
         OR p_column_name LIKE '%AMT%'
         OR p_column_name LIKE '%FEE%'
         OR p_column_name LIKE '%RATE%'
      THEN
        RETURN 'double?';
      
        -- C# "int".
      ELSIF (p_data_precision IS NULL AND p_data_scale IS NULL) -- NUMBER
            OR (p_data_precision BETWEEN 0 AND 9 AND p_data_scale = 0) -- NUMBER(9)
      
      THEN
        RETURN 'int?';
      
      ELSIF (p_data_precision BETWEEN 10 AND 18 AND p_data_scale = 0) -- NUMBER(16)
      THEN
        RETURN 'Int64?';
      
      END IF;
    
    ELSIF p_data_type = 'DATE'
          OR p_data_type = 'TIMESTAMP'
    THEN
      RETURN 'DateTime?';
    ELSE
      -- Rare case, C# Compiler will most likely show this as a syntax error.
      RETURN p_data_type;
    END IF;
  END;

  /*  PROCEDURE postman(p_table_name VARCHAR2) IS
  END;*/

  PROCEDURE routes(p_screen_name VARCHAR2) IS
    v_claims CLOB;
    v_routes CLOB;
  BEGIN
    v_claims := 'insert into t_claims values (GETDATE(),GETDATE(),''' ||
                initcap(pkg_utils.pluralize(p_screen_name)) ||
                ' Menu'', ''A0XX'', ''XX - SYSTEM - ' ||
                upper(pkg_utils.pluralize(p_screen_name)) || ''');' || n ||
                'insert into t_claims values (GETDATE(),GETDATE(),''Add ' ||
                initcap(p_screen_name) || ''', ''A0XX'', ''XX - SYSTEM - ' ||
                upper(pkg_utils.pluralize(p_screen_name)) || ''');' || n ||
                'insert into t_claims values (GETDATE(),GETDATE(),''Edit ' ||
                initcap(p_screen_name) || ''', ''A0XX'', ''XX - SYSTEM - ' ||
                upper(pkg_utils.pluralize(p_screen_name)) || ''');' || n ||
                'insert into t_claims values (GETDATE(),GETDATE(),''Delete ' ||
                initcap(p_screen_name) || ''', ''A0XX'', ''XX - SYSTEM - ' ||
                upper(pkg_utils.pluralize(p_screen_name)) || ''');' || n ||
                'insert into t_claims values (GETDATE(),GETDATE(),''Change ' ||
                initcap(p_screen_name) ||
                ' Status'', ''A0XX'', ''XX - SYSTEM - ' ||
                upper(pkg_utils.pluralize(p_screen_name)) || ''');' || n ||
                '---------------------------------------' || n;
    v_routes := 'insert into t_route values (GETDATE(),GETDATE(),''/api/dashboard/get' ||
                lower(pkg_utils.pluralize(p_screen_name)) || ''', ''Y'');' || n ||
                'insert into t_route values (GETDATE(),GETDATE(),''/api/dashboard/get' ||
                lower(p_screen_name) || ''', ''Y'');' || n ||
                'insert into t_route values (GETDATE(),GETDATE(),''/api/dashboard/add' ||
                lower(p_screen_name) || ''', ''Y'');' || n ||
                'insert into t_route values (GETDATE(),GETDATE(),''/api/dashboard/edit' ||
                lower(p_screen_name) || ''', ''Y'');' || n ||
                'insert into t_route values (GETDATE(),GETDATE(),''/api/dashboard/delete' ||
                lower(p_screen_name) || ''', ''Y'');' || n ||
                'insert into t_route values (GETDATE(),GETDATE(),''/api/dashboard/change' ||
                lower(p_screen_name) || 'status'', ''Y'');' || n;
  
    dbms_output.put_line(v_claims || v_routes);
  END;

END pkg_code_generator;
/
