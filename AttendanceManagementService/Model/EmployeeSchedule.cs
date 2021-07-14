using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceManagementService.Model
{
    public class EmployeeScheduleRequest
    {
       public int       shift_id                      {get;set;}
       public string    encrypt_shift_id              {get;set;}
       public int       employee_id                   {get;set;}
       public string    date_from                     {get;set;}
       public string    date_to                       {get;set;}
       public int       grace_period                  {get;set;}
       public bool      is_flexi                      {get;set;}
       public string    time_in                       {get;set;}
       public string    time_out                      {get;set;}
       public int       time_out_days_cover           {get;set;}
       public decimal   total_working_hours           {get;set;}
       public string    half_day_in                   {get;set;}
       public int       half_day_in_days_cover        {get;set;}
       public string    half_day_out                  {get;set;}
       public int       half_day_out_days_cover       {get;set;}
       public string    night_dif_in                  {get;set;}
       public int       night_dif_in_days_cover       {get;set;}
       public string    night_dif_out                 {get;set;}
       public int       night_dif_out_days_cover      {get;set;}
       public string    first_break_in                {get;set;}
       public int       first_break_in_days_cover     {get;set;}
       public string    first_break_out               {get;set;}
       public int       first_break_out_days_cover    {get;set;}
       public string    second_break_in               {get;set;}
       public int       second_break_in_days_cover    {get;set;}
       public string    second_break_out              {get;set;}
       public int       second_break_out_days_cover   {get;set;}  
       public string    third_break_in                {get;set;}
       public int       third_break_in_days_cover     { get;set;}
       public string    third_break_out               {get;set;}
       public int       third_break_out_days_cover    {get;set;}
       public string    created_by                    {get;set;}
       public string    series_code                    {get;set;}
    }

    public class EmployeeScheduleResponse
    {
       public int       shift_id                      {get;set;}
       public string    encrypt_shift_id              {get;set;}
       public int       employee_id                   {get;set;}
       public string    shift_code                          {get;set;}
       public string    shift_name                          {get;set;}
       public string    date                          {get;set;}
       public int       grace_period                  {get;set;}
       public bool      is_flexi                      {get;set;}
       public string    time_in                       {get;set;}
       public string    time_out                      {get;set;}
       public int       time_out_days_cover           {get;set;}
       public decimal   total_working_hours           {get;set;}
       public string    half_day_in                   {get;set;}
       public int       half_day_in_days_cover        {get;set;}
       public string    half_day_out                  {get;set;}
       public int       half_day_out_days_cover       {get;set;}
       public string    night_dif_in                  {get;set;}
       public int       night_dif_in_days_cover       {get;set;}
       public string    night_dif_out                 {get;set;}
       public int       night_dif_out_days_cover      {get;set;}
       public string    first_break_in                {get;set;}
       public int       first_break_in_days_cover     {get;set;}
       public string    first_break_out               {get;set;}
       public int       first_break_out_days_cover    {get;set;}
       public string    second_break_in               {get;set;}
       public int       second_break_in_days_cover    {get;set;}
       public string    second_break_out              {get;set;}
       public int       second_break_out_days_cover   {get;set;}  
       public string    third_break_in                {get;set;}
       public int       third_break_in_days_cover     { get;set;}
       public string    third_break_out               {get;set;}
       public int       third_break_out_days_cover    {get;set;}
       public int       created_by                    {get;set;}
       public string    date_created                  {get;set;}
    }


    public class EmployeeScheduleDetailRequest
    {
          public int       shift_id                  {get;set;}
          public string    encrypt_shift_id          {get;set;}
          public int       employee_id               {get;set; }
          public string    date_from                 { get; set; }
          public string    date_to                   { get; set; }

          public string		shift_code					{ get; set; }
	      public string		shift_name					{ get; set; }
	      public int		shift_code_type				{ get; set; }
	      public int		grace_period				{ get; set; }
	      public string		description					{ get; set; }
	      public bool		is_flexi					{ get; set; }
	      public string		time_in						{ get; set; }
	      public string		time_out					{ get; set; }
	      public int		time_out_days_cover			{ get; set; }
	      public decimal	total_working_hours			{ get; set; }
	      public bool		is_rd_mon				    { get; set; }
	      public bool		is_rd_tue				    { get; set; }
	      public bool		is_rd_wed				    { get; set; }
	      public bool		is_rd_thu				    { get; set; }
	      public bool		is_rd_fri				    { get; set; }
	      public bool		is_rd_sat				    { get; set; }
	      public bool		is_rd_sun				    { get; set; }		
	      public string		half_day_in					{ get; set; }
	      public int		half_day_in_days_cover		{ get; set; }
	      public string		half_day_out				{ get; set; }
	      public int		half_day_out_days_cover		{ get; set; }
	      public string		night_dif_in				{ get; set; }
	      public int		night_dif_in_days_cover		{ get; set; }
	      public string		night_dif_out				{ get; set; }
	      public int		night_dif_out_days_cover	{ get; set; }
	      public string		first_break_in				{ get; set; }
	      public int		first_break_in_days_cover	{ get; set; }
	      public string		first_break_out				{ get; set; }
	      public int		first_break_out_days_cover	{ get; set; }
	      public string		second_break_in				{ get; set; }
	      public int		second_break_in_days_cover	{ get; set; }
	      public string		second_break_out			{ get; set; }
	      public int		second_break_out_days_cover { get; set; }
	      public string		third_break_in				{ get; set; }
	      public int		third_break_in_days_cover	{ get; set; }
	      public string		third_break_out				{ get; set; }
	      public int		third_break_out_days_cover	{ get; set; }

          public string    created_by                {get;set;}
          public string    series_code                {get;set;}
    }
    public class EmployeeScheduleDetailResponse
    {
          public int       shift_id                  {get;set;}
          public string    encrypt_shift_id          {get;set;}
          public int       employee_id               {get;set; }
          public string    date_from                 { get; set; }
          public string    date_to                    { get; set; }
          public string    date_created              {get;set;}
          public bool       process                  {get;set;}
          public int        created_by                {get;set;}
          public string    series_code                {get;set;}
    }
}
