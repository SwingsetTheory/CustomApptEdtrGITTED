﻿using System;
using System.Collections.Generic;

using Xamarin.Forms;
using System.Diagnostics;

namespace AdditionalAttribute
{
	public partial class EditorLayout : StackLayout
	{
		private Meeting selectedAppointment;
		private DateTime selectedDate;
		public EditorLayout()
		{
			InitializeComponent();
			AddEditorElements();
			saveButton.Clicked += SaveButton_Clicked;
			cancelButton.Clicked += CancelButton_Clicked;
			switchAllDay.Toggled += SwitchAllDay_Toggled;
		}

		private void SwitchAllDay_Toggled(object sender, ToggledEventArgs e)
		{
			if ((sender as Switch).IsToggled)
			{
				startTime_picker.Time = new TimeSpan(12, 0, 0);
				startTime_picker.IsEnabled = false;
				endTime_picker.Time = new TimeSpan(12, 0, 0);
				endTime_picker.IsEnabled = false;
			}
			else
			{
				startTime_picker.IsEnabled = true;
				endTime_picker.IsEnabled = true;
				(sender as Switch).IsToggled = false;
			}
		}
		private void CancelButton_Clicked(object sender, EventArgs e)
		{
			ScheduleAppointmentModifiedEventArgs args = new ScheduleAppointmentModifiedEventArgs();
			args.Appointment = null;
			args.IsModified = false;
			(this.BindingContext as EditorLayoutViewModel).OnAppointmentModified(args);
			this.IsVisible = false;
		}

		private void SaveButton_Clicked(object sender, EventArgs e)
		{
			var endDate = endDate_picker.Date;
			var startDate = startDate_picker.Date;
			var endTime = endTime_picker.Time;
			var startTime = startTime_picker.Time;

			if (endDate < startDate)
			{
				Application.Current.MainPage.DisplayAlert("", "End time should be greater than start time", "OK");
			}
			else if (endDate == startDate)
			{
				if (endTime < startTime)
				{
					Application.Current.MainPage.DisplayAlert("", "End time should be greater than start time", "OK");
				}
				else
				{
					AppointmentDetails();
					this.IsVisible = false;
				}
			}
			else
			{
				AppointmentDetails();
				this.IsVisible = false;
			}
            Debug.WriteLine("EditorLayout: end of SaveButton_Clicked");
		}

		private void AppointmentDetails()
		{
			if (selectedAppointment == null)
			{
				selectedAppointment = new Meeting();
				selectedAppointment.color = Color.FromHex("#5EDAF2");
			}
			if (eventNameText.Text != null)
			{
				selectedAppointment.EventName = eventNameText.Text.ToString();
				if (string.IsNullOrEmpty(selectedAppointment.EventName))
					selectedAppointment.EventName = "Untitled";
			}
			if (organizerText.Text != null)
			{
				selectedAppointment.Organizer = organizerText.Text.ToString();
			}
			selectedAppointment.From = startDate_picker.Date.Add(startTime_picker.Time);
			selectedAppointment.To = endDate_picker.Date.Add(endTime_picker.Time);
			selectedAppointment.IsAllDay = switchAllDay.IsToggled;
			ScheduleAppointmentModifiedEventArgs args = new ScheduleAppointmentModifiedEventArgs();
			args.Appointment = selectedAppointment;
			args.IsModified = true;
			(this.BindingContext as EditorLayoutViewModel).OnAppointmentModified(args);
		}

		public void OpenEditor(Meeting appointment, DateTime date)
		{
			cancelButton.BackgroundColor = Color.FromHex("#E5E5E5");
			saveButton.BackgroundColor = Color.FromHex("#2196F3");
			eventNameText.Placeholder = "Event name";
			organizerText.Placeholder = "Organizer";
			selectedAppointment = null;
			if (appointment != null)
			{
				selectedAppointment = appointment;
				selectedDate = date;
			}
			else
			{
				selectedDate = date;
			}
			UpdateEditor();

		}
		private void UpdateEditor()
		{
			if (selectedAppointment != null)
			{
				eventNameText.Text = selectedAppointment.EventName.ToString();
				organizerText.Text = selectedAppointment.Organizer;
				startDate_picker.Date = selectedAppointment.From;
				endDate_picker.Date = selectedAppointment.To;

				if (!selectedAppointment.IsAllDay)
				{
					startTime_picker.Time = new TimeSpan(selectedAppointment.From.Hour, selectedAppointment.From.Minute, selectedAppointment.From.Second);
					endTime_picker.Time = new TimeSpan(selectedAppointment.To.Hour, selectedAppointment.To.Minute, selectedAppointment.To.Second);
					switchAllDay.IsToggled = false;
				}
				else
				{
					startTime_picker.Time = new TimeSpan(12, 0, 0);
					startTime_picker.IsEnabled = false;
					endTime_picker.Time = new TimeSpan(12, 0, 0);
					endTime_picker.IsEnabled = false;
					switchAllDay.IsToggled = true;
				}
			}
			else
			{
				eventNameText.Text = "";
				organizerText.Text = "";
				switchAllDay.IsToggled = false;
				startDate_picker.Date = selectedDate;
				startTime_picker.Time = new TimeSpan(selectedDate.Hour, selectedDate.Minute, selectedDate.Second);
				endDate_picker.Date = selectedDate;
				endTime_picker.Time = new TimeSpan(selectedDate.Hour + 1, selectedDate.Minute, selectedDate.Second);
			}
		}
		private void AddEditorElements()
		{
			if (Device.RuntimePlatform == "Windows" && Device.Idiom == TargetIdiom.Phone)
			{
				StartdateTimePicker_layout.Padding = new Thickness(20, 0, 20, 0);
				StartdateTimePicker_layout.ColumnDefinitions.Clear();
				StartdateTimePicker_layout.RowDefinitions = new RowDefinitionCollection()
				{
					new RowDefinition { Height = GridLength.Auto },
					new RowDefinition { Height = GridLength.Auto },
				};
				Grid.SetRow(start_datepicker_layout, 0);
				Grid.SetColumnSpan(start_datepicker_layout, 3);
				Grid.SetColumn(start_timepicker_layout, 0);
				Grid.SetColumnSpan(start_timepicker_layout, 3);
				Grid.SetRow(start_timepicker_layout, 1);
				start_datepicker_layout.HeightRequest = 40;
				start_timepicker_layout.HeightRequest = 40;
				startTimeLabel_layout.Padding = new Thickness(20, 10, 0, 0);
				StartdateTimePicker_layout.Padding = new Thickness(20, 0, 20, 0);

				EndDateTimePicker_layout.ColumnDefinitions.Clear();
				EndDateTimePicker_layout.Padding = new Thickness(20, 0, 20, 0);
				EndDateTimePicker_layout.RowDefinitions = new RowDefinitionCollection()
				{
					new RowDefinition { Height = GridLength.Auto },
					new RowDefinition { Height = GridLength.Auto },
				};
				Grid.SetRow(end_datepicker_layout, 0);
				Grid.SetRow(end_timepicker_layout, 1);
				Grid.SetColumnSpan(end_datepicker_layout, 3);
				Grid.SetColumn(end_timepicker_layout, 0);
				Grid.SetColumnSpan(end_timepicker_layout, 3);
				end_datepicker_layout.HeightRequest = 40;
				end_timepicker_layout.HeightRequest = 40;
				endTimeLabel_layout.Padding = new Thickness(20, 10, 0, 0);
			}
			else if (Device.RuntimePlatform == "Android")
			{
				this.Padding = 20;
			}
		}
	}
}
