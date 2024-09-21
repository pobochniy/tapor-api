namespace Tapor.DB.Scripts.Issue;

public static partial class Sql
{
    public static string IssueCreate() =>
        @"insert into Issue (
                   `Assignee`, 
                   Reporter, 
                   Summary, 
                   Description, 
                   Type, 
                   Status, 
                   Priority, 
                   Size, 
                   EstimatedTime, 
                   CreateDate, 
                   DueDate)
                    values (
                        @assignee, 
                        @reporter, 
                        @summary, 
                        @description, 
                        @type, 
                        @status, 
                        @priority,
                        @size,
                        @estimatedTime,
                        @createDate,
                        @dueDate);
                select LAST_INSERT_ID();
            ";
}