using System.ComponentModel.DataAnnotations;
using Tapor.Shared.Enums;

namespace Tapor.DB.Entity;

public class Epic
{
    /// <summary>
    /// ID Эпика
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Исполнитель
    /// </summary>
    public Guid? Assignee { get; set; }

    /// <summary>
    /// Инициатор
    /// </summary>
    public Guid? Reporter { get; set; }

    /// <summary>
    /// Приоритет
    /// </summary>
    public IssuePriorityEnum Priority { get; set; }

    /// <summary>
    /// Название
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Описание
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Время
    /// </summary>
    public DateTime? DueDate { get; set; }
}