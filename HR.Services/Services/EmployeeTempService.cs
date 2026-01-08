// HR.Web.Admin.Services/EmployeeTempService.cs
using HR.Data.Models.BANKING;
using HR.Data.Models.Employees;
using HR.Data.Models.County;
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
				Status = "Active",
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
	}
}
