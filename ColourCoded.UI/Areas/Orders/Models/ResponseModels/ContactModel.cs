﻿using System;
using System.ComponentModel.DataAnnotations;

namespace ColourCoded.UI.Areas.Orders.Models.ResponseModels
{
  public class ContactModel
  {
    public int CustomerId { get; set; }
    public int ContactId { get; set; }
    public string ContactName { get; set; }
    public string ContactNo { get; set; }
    public string EmailAddress { get; set; }
    public string CreateUser { get; set; }
    public DateTime CreateDate { get; set; }
  }
}