
namespace Unfuddle.RestClient.Models
{
	public class People
	{
		public string AccessKey { get; set; }
		/*
  <created-at type="datetime"> </created-at>
  <default-ticket-report-id type="integer"> </default-ticket-report-id>
  <default-time-zone> <!-- i.e. "Pacific Time (US & Canada)" --> </default-time-zone>
  <description> </description>
  <disk-usage type="integer"> <!-- in Kilobytes --> </disk-usage>
  <id type="integer"> </id>
  <plan> [private, micro, compact, corporate, enterprise] </plan>
  <subdomain> </subdomain>
  <!-- a list with valid markups; the first value is the default one -->
  <text-markup> [markdown, textile, plain] </text-markup> 
  <title> </title>
  <updated-at type="datetime"> </updated-at>
  
  <!-- the following is read-only and describes the feature set of an account -->
  <features>
	<attachments type="boolean"> [true, false] </attachments>
	<ssl type="boolean"> [true, false] </ssl>
	<storage type="integer"> <!-- in Megabytes --> </storage>
	<time-tracking type="boolean"> [true, false] </time-tracking>
  </features>
</account>
		*/
	}
}