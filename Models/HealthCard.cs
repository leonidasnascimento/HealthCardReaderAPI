﻿using API.Patterns;
using System;
using System.Drawing;

namespace API.Models
{
    /// <summary>
    /// the class HealthCare is responsible for establishing the atributes that are commom for all the health cards
    /// </summary>
    public abstract class HealthCard
    {
        /// <summary>
        /// Name represents the healthcard's owner
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// CardNumber representes the number of the card
        /// </summary>
        public string CardNumber { get; protected set; }

        /// <summary>
        /// ValidDate represents the last date the card will be valid
        /// </summary>
        public DateTime ValidDate { get; protected set; }

        /// <summary>
        /// HealthInsurance represents the type of the Health Insurance
        /// </summary>
        public string HealthInsurance { get; protected set; }

        /// <summary>
        /// Logo represents which is the medical agreement of the card
        /// </summary>
        public Image Logo { get; protected set; }
        /// <summary>
        /// Company represents the company in which the owner of the card works
        /// </summary>
        public string Company { get; protected set; }

        public abstract HealthCard ReadCardInfo(string json);
    }
}