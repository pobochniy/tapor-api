using System.ComponentModel.DataAnnotations;

namespace Tapor.Api.Dtos;

/// <summary>
/// Хотелка заказчика
/// </summary>
public class IssueDto
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public long? Id { get; set; }
    
    /// <summary>
    /// Исполнитель
    /// </summary>
    public Guid? Assignee { get; set; }
    
    /// <summary>
    /// Инициатор
    /// </summary>
    [Required]
    public Guid Reporter { get; set; }
    
    /// <summary>
    /// Сводка
    /// </summary>
    public string Summary { get; set; }
    
    /// <summary>
    /// Описание
    /// </summary>
    public string Description { get; set; }
    
    /// <summary>
    /// Тип
    /// </summary>
    public int Type { get; set; }
    
    /// <summary>
    /// Статус
    /// </summary>
    public int Status { get; set; }
    
    /// <summary>
    /// Приоритет
    /// </summary>
    public int Priority { get; set; }
    
    /// <summary>
    /// Условный размер задачи
    /// </summary>
    public int Size { get; set; }
    
    /// <summary>
    /// Предполагаемое время на задачу
    /// </summary>
    public decimal? EstimatedTime { get; set; }
    
    /// <summary>
    /// Дата крайнего срока завершения события
    /// </summary>
    public DateTime? DueDate { get; set; }
}