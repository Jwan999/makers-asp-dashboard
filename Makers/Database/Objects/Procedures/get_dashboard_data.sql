USE [db_aa340a_makers]
GO
/****** Object:  StoredProcedure [dbo].[get_dashboard_data]    Script Date: 1/24/2024 6:56:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Create the stored procedure
ALTER   PROCEDURE [dbo].[get_dashboard_data]
    @proj NVARCHAR(MAX),
    @OutputData NVARCHAR(MAX) OUTPUT

AS
BEGIN
    DECLARE @Data NVARCHAR(MAX);

	if @proj  = 'All'
	begin
     SELECT @OutputData = (
 select * from   (SELECT 'Total Projects' AS [TYPE],
          '' AS [KEY],
              COUNT(*) AS [VALUE],
		      '' AS [KEY2],
		      ''  as  [VALUE2],
			  '' AS [KEY3],
		      ''  as  [VALUE3]
    FROM T_PROJECTS t
        UNION
       SELECT 'Total Training' AS [TYPE],
             '' AS [KEY],
              COUNT(*) AS [VALUE],
		      '' AS [KEY2],
		      ''  as  [VALUE2],
			  '' AS [KEY3],
		      ''  as  [VALUE3]
    FROM T_TRAINING t

	   UNION
       SELECT 'Total Partners' AS [TYPE],
              '' AS [KEY],
              COUNT(*) AS [VALUE],
		      '' AS [KEY2],
		      ''  as  [VALUE2],
			  '' AS [KEY3],
		      ''  as  [VALUE3]
    FROM T_INST t
	  UNION
       SELECT 'Number of Trainers' AS [TYPE],
              '' AS [KEY],
              COUNT(*) AS [VALUE],
		      '' AS [KEY2],
		      ''  as  [VALUE2],
			  '' AS [KEY3],
		      ''  as  [VALUE3]
    FROM T_TRAINERS t
	 UNION
       SELECT 'Total Sessions' AS [TYPE],
              '' AS [KEY],
               COUNT(*) AS [VALUE],
		      '' AS [KEY2],
		      ''  as  [VALUE2],
			  '' AS [KEY3],
		      ''  as  [VALUE3]
     FROM T_TRAINING t where t.TYPEX = 'SESSION'
	 UNION
       SELECT 'Number of Workshops' AS [TYPE],
             '' AS [KEY],
             COUNT(*) AS [VALUE],
		      '' AS [KEY2],
		      ''  as  [VALUE2],
			  '' AS [KEY3],
		      ''  as  [VALUE3]
    FROM T_TRAINING t where t.TYPEX = 'WORKSHOP'
     UNION
       SELECT 'Number of Courses' AS [TYPE],
             '' AS [KEY],
             COUNT(*) AS [VALUE],
		      '' AS [KEY2],
		      ''  as  [VALUE2],
			  '' AS [KEY3],
		      ''  as  [VALUE3]
    FROM T_TRAINING t where t.TYPEX = 'COURSE'
	  UNION
       SELECT 'Number of Students' AS [TYPE],
                '' AS [KEY],
             '60' AS [VALUE],
		      '' AS [KEY2],
		      ''  as  [VALUE2],
			  '' AS [KEY3],
		      ''  as  [VALUE3]
	  UNION
       SELECT 'Number of New Students' AS [TYPE],
               '' AS [KEY],
             '10' AS [VALUE],
		      '' AS [KEY2],
		      ''  as  [VALUE2],
			  '' AS [KEY3],
		      ''  as  [VALUE3]
	   UNION
       SELECT 'Number of Interns' AS [TYPE],
                '' AS [KEY1],
             '10' AS [VALUE],
		      '' AS [KEY2],
		      ''  as  [VALUE2],
			  '' AS [KEY3],
		      ''  as  [VALUE3]
		   UNION
       SELECT 'Number of New Interns' AS [TYPE],
                 '' AS [KEY],
             '5' AS [VALUE],
		      '' AS [KEY2],
		      ''  as  [VALUE2],
			  '' AS [KEY3],
		      ''  as  [VALUE3]
				   UNION
       SELECT 'Number of New Members' AS [TYPE],
                  '' AS [KEY],
             '50' AS [VALUE],
		      '' AS [KEY2],
		      ''  as  [VALUE2],
			  '' AS [KEY3],
		      ''  as  [VALUE3]
		   		   UNION
       SELECT 'Gender' AS [TYPE],
            'Male' as [KEY],
            '52' AS [VALUE],
		    'Female' as [KEY2],
		    '48'  as  [VALUE2],
			 '' AS [KEY3],
		     ''  as  [VALUE3]
			    UNION
       SELECT 'Age Range' AS [TYPE],
            'Percentage of 15-19' as [KEY],
            '30' AS [VALUE],
		    'Percentage of 19-25' as [KEY2],
		    '50'  as  [VALUE2],
			 'Percentage of 25-35' AS [KEY3],
		     '20'  as  [VALUE3]
			 UNION
   SELECT
    'Statistics' AS [TYPE],
    FORMAT(t.INSDATE, 'yyyy/MM') AS [KEY],
    SUM(CASE WHEN typex = 'COURSE' THEN 1 ELSE 0 END) AS [VALUE] ,
    FORMAT(t.INSDATE, 'yyyy/MM') AS [KEY2],
    SUM(CASE WHEN typex = 'WORKSHOP' THEN 1 ELSE 0 END) AS [VALUE2],
	FORMAT(t.INSDATE, 'yyyy/MM') AS [KEY3],
    SUM(CASE WHEN typex = 'SESSION' THEN 1 ELSE 0 END) AS [VALUE3]
FROM
    t_training t
GROUP BY
    FORMAT(INSDATE, 'yyyy/MM')
ORDER BY FORMAT(t.INSDATE, 'yyyy/MM') OFFSET 0 ROWS
) AS result

	for json path);
	end ; 

	else 
	begin 
	     SELECT @OutputData = (
 select * from   (SELECT 'Total Projects' AS [TYPE],
          '' AS [KEY],
              COUNT(*) AS [VALUE],
		      '' AS [KEY2],
		      ''  as  [VALUE2],
			  '' AS [KEY3],
		      ''  as  [VALUE3]
    FROM T_PROJECTS t where t.NAMEX = @proj


        UNION
       SELECT 'Total Training' AS [TYPE],
             '' AS [KEY],
              COUNT(*) AS [VALUE],
		      '' AS [KEY2],
		      ''  as  [VALUE2],
			  '' AS [KEY3],
		      ''  as  [VALUE3]
    FROM T_TRAINING t where t.PROJECT = @proj
	 UNION
       SELECT 'Total Sessions' AS [TYPE],
              '' AS [KEY],
               COUNT(*) AS [VALUE],
		      '' AS [KEY2],
		      ''  as  [VALUE2],
			  '' AS [KEY3],
		      ''  as  [VALUE3]
     FROM T_TRAINING t where t.TYPEX = 'SESSION' and t.PROJECT = @proj
	 UNION
       SELECT 'Number of Workshops' AS [TYPE],
             '' AS [KEY],
             COUNT(*) AS [VALUE],
		      '' AS [KEY2],
		      ''  as  [VALUE2],
			  '' AS [KEY3],
		      ''  as  [VALUE3]
    FROM T_TRAINING t where t.TYPEX = 'WORKSHOP' and t.PROJECT = @proj
     UNION
       SELECT 'Number of Courses' AS [TYPE],
             '' AS [KEY],
             COUNT(*) AS [VALUE],
		      '' AS [KEY2],
		      ''  as  [VALUE2],
			  '' AS [KEY3],
		      ''  as  [VALUE3]
    FROM T_TRAINING t where t.TYPEX = 'COURSE' and t.PROJECT = @proj
	  UNION
       SELECT 'Number of Students' AS [TYPE],
                '' AS [KEY],
             '60' AS [VALUE],
		      '' AS [KEY2],
		      ''  as  [VALUE2],
			  '' AS [KEY3],
		      ''  as  [VALUE3]
	  UNION
       SELECT 'Number of New Students' AS [TYPE],
               '' AS [KEY],
             '10' AS [VALUE],
		      '' AS [KEY2],
		      ''  as  [VALUE2],
			  '' AS [KEY3],
		      ''  as  [VALUE3]
      UNION
			    SELECT 'Number of Interns' AS [TYPE],
                '' AS [KEY1],
             '10' AS [VALUE],
		      '' AS [KEY2],
		      ''  as  [VALUE2],
			  '' AS [KEY3],
		      ''  as  [VALUE3]
		   UNION
       SELECT 'Number of New Interns' AS [TYPE],
                 '' AS [KEY],
             '5' AS [VALUE],
		      '' AS [KEY2],
		      ''  as  [VALUE2],
			  '' AS [KEY3],
		      ''  as  [VALUE3]
				   
	   UNION
       SELECT 'Number of Trainers' AS [TYPE],
              '' AS [KEY],
             count(DISTINCT mt.trainer_id) AS [VALUE],
		      '' AS [KEY2],
		      ''  as  [VALUE2],
			  '' AS [KEY3],
		      ''  as  [VALUE3]
	  from T_MAP_TRAINING_TRAINERS  mt
	         join  t_training tr
			 on  mt.training_id = tr.id
			 and tr.project =  @proj
	 UNION
      SELECT 'Gender' AS [TYPE],
            'Male' as [KEY],
            '52' AS [VALUE],
		    'Female' as [KEY2],
		    '48'  as  [VALUE2],
			 '' AS [KEY3],
		     ''  as  [VALUE3]
			    UNION
       SELECT 'Age Range' AS [TYPE],
            'Percentage of 15-19' as [KEY],
            '30' AS [VALUE],
		    'Percentage of 19-25' as [KEY2],
		    '50'  as  [VALUE2],
			 'Percentage of 25-35' AS [KEY3],
		     '20'  as  [VALUE3]
			 UNION
   SELECT
    'Statistics' AS [TYPE],
    FORMAT(t.INSDATE, 'yyyy/MM') AS [KEY],
    SUM(CASE WHEN typex = 'COURSE' and  PROJECT = @proj THEN 1 ELSE 0 END) AS [VALUE] ,
    FORMAT(t.INSDATE, 'yyyy/MM') AS [KEY2],
    SUM(CASE WHEN typex = 'WORKSHOP' and  PROJECT = @proj THEN 1 ELSE 0 END) AS [VALUE2],
	FORMAT(t.INSDATE, 'yyyy/MM') AS [KEY3],
    SUM(CASE WHEN typex = 'SESSION' and  PROJECT = @proj THEN 1 ELSE 0 END) AS [VALUE3]
FROM
    t_training t
GROUP BY
    FORMAT(INSDATE, 'yyyy/MM')
ORDER BY FORMAT(t.INSDATE, 'yyyy/MM') OFFSET 0 ROWS
) AS result

	for json path);
	end ; 
END;