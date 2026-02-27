// HR.Web.Admin.Services/EmployeeTempService.cs
using HR.Core.Enums;
using HR.Data.Models.BANKING;
using HR.Data.Models.County;
using HR.Data.Models.Employees;
using HR.Data.Models.Recruitment;
using Microsoft.AspNetCore.Builder;

using System;
using System.Collections.Generic;
using System.Linq;

namespace HR.Services.Services
{
	public class EmployeeTempService
	{
		public Employee TempEmployee { get; private set; } = null!;

		public EmployeeTempService()
		{
			Reset();
		}

		
		/// This mirrors the fields used across Create -> Additional -> WorkHistory -> NHS -> BankPayment -> Confirm.
		/// </summary>
		public void Reset()
		{
			TempEmployee = new Employee
			{
				Id = Guid.NewGuid(),

				// Basic scalar fields (ensure not-null defaults so bindings don't get unexpected nulls)
				EmployeeCode = string.Empty,
				FirstName = string.Empty,
				MiddleName = string.Empty,
				LastName = string.Empty,
				Email = string.Empty,
				Phone = string.Empty,
				CountyId = default,      // Start with no selection
				SubCountyId = default,   // Start with no selection
				Estate = string.Empty,
				POBox = string.Empty,
				NationalID = string.Empty,
				Gender = string.Empty,
				JobTitle = string.Empty,
				Status = EmployeeStatus.Active,
				ContractType = ContractType.Permanent,
				Disability = string.Empty,

				// Dates - keep nullable if model is nullable; otherwise default to Today or MinValue
				DOB = DateTime.Today,
				HireDate = DateTime.Today,

				//Ethnicity relation 
				EthnicityId = default,

				// Department relation 
				DepartmentId = default,

				// Collections used by multi-step pages
				Education = new List<EducationHistory>(),
				WorkHistory = new List<WorkHistory>(),
				NextOfKin = new List<NextOfKin>(),
				Hobbies = new List<Hobby>(),
				Skills = new List<Skill>(),

				// Payment/Bank objects
				BankDetail = new BankDetail(),
				PaymentData = new PaymentData(),

				DisabilityDetail = new DisabilityDetail()
			};
		}

		public void Clear() => Reset();

		// ---------- Helpers for working with lists from pages ----------
		// ---------- NEW: Helper for Disability Details ----------
		public void SetDisabilityDetail(DisabilityDetail detail)
		{
			if (detail == null) return;

			detail.EmployeeId = TempEmployee.Id;
			TempEmployee.DisabilityDetail = detail;
		}

		// Education
		public void AddEducation(EducationHistory edu)
		{
			if (edu == null) return;
			edu.EmployeeId = TempEmployee.Id;
			TempEmployee.Education.Add(edu);
		}
		public void UpdateEducation(int index, EducationHistory edu)
		{
			if (index < 0 || index >= TempEmployee.Education.Count) return;
			edu.EmployeeId = TempEmployee.Id;
			TempEmployee.Education[index] = edu;
		}
		public void RemoveEducation(int index)
		{
			if (index < 0 || index >= TempEmployee.Education.Count) return;
			TempEmployee.Education.RemoveAt(index);
		}

		// Work history
		public void AddWork(WorkHistory work)
		{
			if (work == null) return;
			work.EmployeeId = TempEmployee.Id;
			TempEmployee.WorkHistory.Add(work);
		}
		public void UpdateWork(int index, WorkHistory work)
		{
			if (index < 0 || index >= TempEmployee.WorkHistory.Count) return;
			work.EmployeeId = TempEmployee.Id;
			TempEmployee.WorkHistory[index] = work;
		}
		public void RemoveWork(int index)
		{
			if (index < 0 || index >= TempEmployee.WorkHistory.Count) return;
			TempEmployee.WorkHistory.RemoveAt(index);
		}

		// Next of kin
		public void AddNextOfKin(NextOfKin nok)
		{
			if (nok == null) return;
			nok.EmployeeId = TempEmployee.Id;
			TempEmployee.NextOfKin.Add(nok);
		}
		public void UpdateNextOfKin(int index, NextOfKin nok)
		{
			if (index < 0 || index >= TempEmployee.NextOfKin.Count) return;
			nok.EmployeeId = TempEmployee.Id;
			TempEmployee.NextOfKin[index] = nok;
		}
		public void RemoveNextOfKin(int index)
		{
			if (index < 0 || index >= TempEmployee.NextOfKin.Count) return;
			TempEmployee.NextOfKin.RemoveAt(index);
		}

		// Hobbies & Skills
		public void AddHobby(Hobby h) { if (h == null) return; TempEmployee.Hobbies.Add(h); }
		public void RemoveHobby(int index) { if (index < 0 || index >= TempEmployee.Hobbies.Count) return; TempEmployee.Hobbies.RemoveAt(index); }
		public void AddSkill(Skill s) { if (s == null) return; TempEmployee.Skills.Add(s); }
		public void RemoveSkill(int index) { if (index < 0 || index >= TempEmployee.Skills.Count) return; TempEmployee.Skills.RemoveAt(index); }

		// Bank & Payment
		public void SetBankDetail(BankDetail bank)
		{
			if (bank == null) return;

			// Ensure the BankDetail has an ID before PaymentData references it
			if (bank.Id == Guid.Empty)
			{
				bank.Id = Guid.NewGuid(); // Assign a temporary GUID immediately
			}

			bank.EmployeeId = TempEmployee.Id;
			TempEmployee.BankDetail = bank;
			// CRITICAL: If PaymentData exists, update its FK
			if (TempEmployee.PaymentData != null)
			{
				TempEmployee.PaymentData.BankDetailId = bank.Id;
			}
		}

		public void SetPaymentData(PaymentData payment)
		{
			if (payment == null) return;

			// NEW LOGIC: PaymentData now requires BankDetailId
			payment.BankDetailId = TempEmployee.BankDetail?.Id ?? Guid.Empty; // Use Guid.Empty if BankDetail hasn't been set yet
			payment.EmployeeId = TempEmployee.Id;
			TempEmployee.PaymentData = payment;
		}

		// Inside HR.Services.Services.EmployeeTempService
		public void InitializeFromApplication(JobApplication app)
		{
			Reset(); // Always start fresh

			// 1. Map Basic Info (Smart Name Split)
			var names = app.FullName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			TempEmployee.FirstName = names.Length > 0 ? names[0] : "";

			if (names.Length == 2)
			{
				TempEmployee.LastName = names[1];
			}
			else if (names.Length >= 3)
			{
				TempEmployee.MiddleName = names[1];
				TempEmployee.LastName = string.Join(" ", names.Skip(2));
			}

			TempEmployee.Email = app.Email;
			TempEmployee.Phone = app.PhoneNumber;
			TempEmployee.JobTitle = app.JobListing?.JobRequisition?.JobTitle ?? app.JobListing?.ExternalTitle;
			TempEmployee.DepartmentId = app.JobListing?.JobRequisition?.DepartmentId ?? Guid.Empty;
			// 2. NEW: Map Location & Address (Directly from Application)
			TempEmployee.CountryId = app.CountryId;
			TempEmployee.CountyId = app.CountyId;
			TempEmployee.SubCountyId = app.SubCountyId;
			TempEmployee.Estate = app.Estate;
			TempEmployee.POBox = app.POBox;

			// 2. Map Education (Linking to your existing EducationHistory model)
			TempEmployee.Education = app.Education.Select(e => new EducationHistory
			{
				Id = Guid.NewGuid(),
				EmployeeId = TempEmployee.Id,
				SchoolName = e.Institution,
				Field = e.Field,
				Level = e.Level,
				Country = e.Country ?? "N/A",
				FromDate = e.StartDate,
				ToDate = e.EndDate ?? DateTime.Today
			}).ToList();

			// 3. Map Work History (Linking to your existing WorkHistory model)
			TempEmployee.WorkHistory = app.Experience.Select(w => new WorkHistory
			{
				Id = Guid.NewGuid(),
				EmployeeId = TempEmployee.Id,
				CompanyName = w.Company,
				JobTitle = w.JobTitle,
				JobDuties = w.Responsibilities,
				JobFromDate = w.StartDate,
				JobToDate = w.EndDate ?? DateTime.Today,
				IsCurrentJob = w.EndDate == null,
				CompanyCity = w.City ?? "N/A",
				CompanyCountry = w.Country ?? "N/A"
			}).ToList();
		}
	}
}
