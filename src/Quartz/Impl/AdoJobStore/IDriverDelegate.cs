/* 
* Copyright 2004-2005 OpenSymphony 
* 
* Licensed under the Apache License, Version 2.0 (the "License"); you may not 
* use this file except in compliance with the License. You may obtain a copy 
* of the License at 
* 
*   http://www.apache.org/licenses/LICENSE-2.0 
*   
* Unless required by applicable law or agreed to in writing, software 
* distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
* WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
* License for the specific language governing permissions and limitations 
* under the License.
* 
*/

/*
* Previously Copyright (c) 2001-2004 James House
*/
using System;
using System.Collections;
using System.Data;

using Quartz.Collection;
using Quartz.Spi;
using Quartz.Util;

namespace Quartz.Impl.AdoJobStore
{
	/// <summary>
	/// This is the base interface for all driver delegate classes.
	/// <p>
	/// This interface is very similar to the <code>IJobStore</code>
	/// interface except each method has an additional <code>Connection}</code>
	/// parameter.
	/// </p>
	/// 
	/// <p>
	/// Unless a database driver has some <strong>extremely-DB-specific</strong>
	/// requirements, any IDriverDelegate implementation classes should extend the
	/// <code>StdAdoDelegate</code> class.
	/// </p>
	/// 
	/// </summary>
	/// <author> <a href="mailto:jeff@binaryfeed.org">Jeffrey Wescott</a> </author>
	/// <author>James House</author>
	/// <author>Marko Lahma (.NET)</author>
	public interface IDriverDelegate
	{

		/// <summary>
		/// Update all triggers having one of the two given states, to the given new
		/// state.
		/// </summary>
		/// <param name="conn">The DB Connection</param>
		/// <param name="newState">The new state for the triggers</param>
		/// <param name="oldState1">The first old state to update</param>
		/// <param name="oldState2">The second old state to update</param>
		/// <returns>Number of rows updated</returns>
		int UpdateTriggerStatesFromOtherStates(IDbConnection conn, string newState, string oldState1, string oldState2);

		/// <summary>
		/// Get the names of all of the triggers that have misfired - according to
		/// the given timestamp.
		/// </summary>
		/// <param name="conn">The DB Connection</param>
		/// <returns>An array of <code>Key</code> objects</returns>
		Key[] SelectMisfiredTriggers(IDbConnection conn, long ts);

		/// <summary>
		/// Get the names of all of the triggers in the given state that have
		/// misfired - according to the given timestamp.
		/// </summary>
		/// <param name="conn">The DB Connection</param>
		/// <returns>An array of <code>Key</code> objects</returns>
		Key[] SelectMisfiredTriggersInState(IDbConnection conn, string state, long ts);

		/// <summary>
		/// Get the names of all of the triggers in the given group and state that
		/// have misfired - according to the given timestamp.
		/// </summary>
		/// <param name="conn">The DB Connection</param>
		/// <returns>An array of <code>Key</code> objects</returns>
		Key[] SelectMisfiredTriggersInGroupInState(IDbConnection conn, string groupName, string state, long ts);

		/// <summary> 
		/// Select all of the triggers for jobs that are requesting recovery. The
		/// returned trigger objects will have unique "recoverXXX" trigger names and
		/// will be in the <code>Scheduler.DEFAULT_RECOVERY_GROUP</code> trigger group.
		///
		/// <p>
		/// In order to preserve the ordering of the triggers, the fire time will be
		/// set from the <code>COL_FIRED_TIME</code> column in the <code>TABLE_FIRED_TRIGGERS</code>
		/// table. The caller is responsible for calling <code>computeFirstFireTime</code>
		/// on each returned trigger. It is also up to the caller to insert the
		/// returned triggers to ensure that they are fired.
		/// </p>
		/// </summary>
		/// <param name="conn">The DB Connection</param>
		/// <returns>An array of <code>Trigger</code> objects</returns>
		Trigger[] SelectTriggersForRecoveringJobs(IDbConnection conn);

		/// <summary>
		/// Delete all fired triggers.
		/// </p>
		/// </summary>
		/// <param name="conn">The DB Connection</param>
		/// <returns>The number of rows deleted</returns>
		int DeleteFiredTriggers(IDbConnection conn);

		/// <summary> 
		/// Delete all fired triggers of the given instance.
		/// </summary>
		/// <param name="conn">The DB Connection</param>
		/// <returns>The number of rows deleted</returns>
		int DeleteFiredTriggers(IDbConnection conn, string instanceId);

		/// <summary>
		/// Delete all volatile fired triggers.
		/// </summary>
		/// <param name="conn">The DB Connection</param>
		/// <returns>The number of rows deleted</returns>
		int DeleteVolatileFiredTriggers(IDbConnection conn);

		/// <summary>
		/// Get the names of all of the triggers that are volatile.
		/// </summary>
		/// <param name="conn">The DB Connection</param>
		/// <returns> an array of <code>{@link
		/// org.quartz.utils.Key}</code> objects
		/// </returns>
		Key[] SelectVolatileTriggers(IDbConnection conn);

		/// <summary> <p>
		/// Get the names of all of the jobs that are volatile.
		/// </p>
		/// 
		/// </summary>
		/// <param name="conn">The DB Connection</param>
		/// <returns> an array of <code>{@link
		/// org.quartz.utils.Key}</code> objects
		/// </returns>
		Key[] SelectVolatileJobs(IDbConnection conn);

		//---------------------------------------------------------------------------
		// jobs
		//---------------------------------------------------------------------------

		/// <summary> <p>
		/// Insert the job detail record.
		/// </p>
		/// 
		/// </summary>
		/// <param name="conn">The DB Connection</param>
		/// <param name="">job
		/// the job to insert
		/// </param>
		/// <returns> number of rows inserted
		/// </returns>
		/// <throws>  IOException </throws>
		/// <summary>           if there were problems serializing the JobDataMap
		/// </summary>
		int InsertJobDetail(IDbConnection conn, JobDetail job);

		/// <summary> <p>
		/// Update the job detail record.
		/// </p>
		/// 
		/// </summary>
		/// <param name="conn">The DB Connection</param>
		/// <param name="">job
		/// the job to update
		/// </param>
		/// <returns> number of rows updated
		/// </returns>
		/// <throws>  IOException </throws>
		/// <summary>           if there were problems serializing the JobDataMap
		/// </summary>
		int UpdateJobDetail(IDbConnection conn, JobDetail job);

		/// <summary> <p>
		/// Get the names of all of the triggers associated with the given job.
		/// </p>
		/// 
		/// </summary>
		/// <param name="conn">The DB Connection</param>
		/// <param name="">jobName
		/// the job name
		/// </param>
		/// <param name="">groupName
		/// the job group
		/// </param>
		/// <returns> an array of <code>{@link
		/// org.quartz.utils.Key}</code> objects
		/// </returns>
		Key[] SelectTriggerNamesForJob(IDbConnection conn, string jobName, string groupName);

		/// <summary> <p>
		/// Delete all job listeners for the given job.
		/// </p>
		/// 
		/// </summary>
		/// <param name="conn">The DB Connection</param>
		/// <param name="">jobName
		/// the name of the job
		/// </param>
		/// <param name="">groupName
		/// the group containing the job
		/// </param>
		/// <returns> the number of rows deleted
		/// </returns>
		int DeleteJobListeners(IDbConnection conn, string jobName, string groupName);

		/// <summary> <p>
		/// Delete the job detail record for the given job.
		/// </p>
		/// 
		/// </summary>
		/// <param name="conn">The DB Connection</param>
		/// <param name="">jobName
		/// the name of the job
		/// </param>
		/// <param name="">groupName
		/// the group containing the job
		/// </param>
		/// <returns> the number of rows deleted
		/// </returns>
		int DeleteJobDetail(IDbConnection conn, string jobName, string groupName);

		/// <summary> <p>
		/// Check whether or not the given job is stateful.
		/// </p>
		/// 
		/// </summary>
		/// <param name="conn">The DB Connection</param>
		/// <param name="">jobName
		/// the name of the job
		/// </param>
		/// <param name="">groupName
		/// the group containing the job
		/// </param>
		/// <returns> true if the job exists and is stateful, false otherwise
		/// </returns>
		
		bool IsJobStateful(IDbConnection conn, string jobName, string groupName);

		/// <summary> <p>
		/// Check whether or not the given job exists.
		/// </p>
		/// 
		/// </summary>
		/// <param name="conn">The DB Connection</param>
		/// <param name="">jobName
		/// the name of the job
		/// </param>
		/// <param name="">groupName
		/// the group containing the job
		/// </param>
		/// <returns> true if the job exists, false otherwise
		/// </returns>
		bool JobExists(IDbConnection conn, string jobName, string groupName);

		/// <summary> <p>
		/// Update the job data map for the given job.
		/// </p>
		/// 
		/// </summary>
		/// <param name="conn">The DB Connection</param>
		/// <param name="">job
		/// the job to update
		/// </param>
		/// <returns> the number of rows updated
		/// </returns>
		/// <throws>  IOException </throws>
		/// <summary>           if there were problems serializing the JobDataMap
		/// </summary>
		int UpdateJobData(IDbConnection conn, JobDetail job);

		/// <summary> <p>
		/// Associate a listener with a job.
		/// </p>
		/// 
		/// </summary>
		/// <param name="conn">The DB Connection</param>
		/// <param name="">job
		/// the job to associate with the listener
		/// </param>
		/// <param name="">listener
		/// the listener to insert
		/// </param>
		/// <returns> the number of rows inserted
		/// </returns>
		int InsertJobListener(IDbConnection conn, JobDetail job, string listener);

		/// <summary> <p>
		/// Get all of the listeners for a given job.
		/// </p>
		/// 
		/// </summary>
		/// <param name="conn">The DB Connection</param>
		/// <param name="">jobName
		/// the job name whose listeners are wanted
		/// </param>
		/// <param name="">groupName
		/// the group containing the job
		/// </param>
		/// <returns> array of <code>String</code> listener names
		/// </returns>
		string[] SelectJobListeners(IDbConnection conn, string jobName, string groupName);

		/// <summary>
		/// Select the JobDetail object for a given job name / group name.
		/// </summary>
		/// <param name="conn">The DB Connection</param>
		/// <param name="">jobName
		/// the job name whose listeners are wanted
		/// </param>
		/// <param name="">groupName
		/// the group containing the job
		/// </param>
		/// <returns> the populated JobDetail object
		/// </returns>
		/// <throws>  ClassNotFoundException </throws>
		/// <summary>           if a class found during deserialization cannot be found or if
		/// the job class could not be found
		/// </summary>
		/// <throws>  IOException </throws>
		/// <summary>           if deserialization causes an error
		/// </summary>
		JobDetail SelectJobDetail(IDbConnection conn, string jobName, string groupName, IClassLoadHelper classLoadHelper);

		/// <summary> <p>
		/// Select the total number of jobs stored.
		/// </p>
		/// 
		/// </summary>
		/// <param name="">conn
		/// the DB Connection
		/// </param>
		/// <returns> the total number of jobs stored
		/// </returns>
		
		int SelectNumJobs(IDbConnection conn);

		/// <summary> <p>
		/// Select all of the job group names that are stored.
		/// </p>
		/// 
		/// </summary>
		/// <param name="">conn
		/// the DB Connection
		/// </param>
		/// <returns> an array of <code>String</code> group names
		/// </returns>
		
		string[] SelectJobGroups(IDbConnection conn);

		/// <summary> <p>
		/// Select all of the jobs contained in a given group.
		/// </p>
		/// 
		/// </summary>
		/// <param name="">conn
		/// the DB Connection
		/// </param>
		/// <param name="">groupName
		/// the group containing the jobs
		/// </param>
		/// <returns> an array of <code>String</code> job names
		/// </returns>
		
		string[] SelectJobsInGroup(IDbConnection conn, string groupName);

		//---------------------------------------------------------------------------
		// triggers
		//---------------------------------------------------------------------------

		/// <summary> <p>
		/// Insert the base trigger data.
		/// </p>
		/// 
		/// </summary>
		/// <param name="">conn
		/// the DB Connection
		/// </param>
		/// <param name="">trigger
		/// the trigger to insert
		/// </param>
		/// <param name="">state
		/// the state that the trigger should be stored in
		/// </param>
		/// <returns> the number of rows inserted
		/// </returns>
		
		int InsertTrigger(IDbConnection conn, Trigger trigger, string state, JobDetail jobDetail);

		/// <summary> <p>
		/// Insert the simple trigger data.
		/// </p>
		/// 
		/// </summary>
		/// <param name="">conn
		/// the DB Connection
		/// </param>
		/// <param name="">trigger
		/// the trigger to insert
		/// </param>
		/// <returns> the number of rows inserted
		/// </returns>
		
		int InsertSimpleTrigger(IDbConnection conn, SimpleTrigger trigger);

		/// <summary> <p>
		/// Insert the blob trigger data.
		/// </p>
		/// 
		/// </summary>
		/// <param name="">conn
		/// the DB Connection
		/// </param>
		/// <param name="">trigger
		/// the trigger to insert
		/// </param>
		/// <returns> the number of rows inserted
		/// </returns>
		
		int InsertBlobTrigger(IDbConnection conn, Trigger trigger);

		/// <summary> <p>
		/// Insert the cron trigger data.
		/// </p>
		/// 
		/// </summary>
		/// <param name="">conn
		/// the DB Connection
		/// </param>
		/// <param name="">trigger
		/// the trigger to insert
		/// </param>
		/// <returns> the number of rows inserted
		/// </returns>
		
		int InsertCronTrigger(IDbConnection conn, CronTrigger trigger);

		/// <summary> <p>
		/// Update the base trigger data.
		/// </p>
		/// 
		/// </summary>
		/// <param name="">conn
		/// the DB Connection
		/// </param>
		/// <param name="">trigger
		/// the trigger to insert
		/// </param>
		/// <param name="">state
		/// the state that the trigger should be stored in
		/// </param>
		/// <returns> the number of rows updated
		/// </returns>
		
		int UpdateTrigger(IDbConnection conn, Trigger trigger, string state, JobDetail jobDetail);

		/// <summary> <p>
		/// Update the simple trigger data.
		/// </p>
		/// 
		/// </summary>
		/// <param name="">conn
		/// the DB Connection
		/// </param>
		/// <param name="">trigger
		/// the trigger to insert
		/// </param>
		/// <returns> the number of rows updated
		/// </returns>
		
		int UpdateSimpleTrigger(IDbConnection conn, SimpleTrigger trigger);

		/// <summary> <p>
		/// Update the cron trigger data.
		/// </p>
		/// 
		/// </summary>
		/// <param name="">conn
		/// the DB Connection
		/// </param>
		/// <param name="">trigger
		/// the trigger to insert
		/// </param>
		/// <returns> the number of rows updated
		/// </returns>
		
		int UpdateCronTrigger(IDbConnection conn, CronTrigger trigger);

		/// <summary> <p>
		/// Update the blob trigger data.
		/// </p>
		/// 
		/// </summary>
		/// <param name="">conn
		/// the DB Connection
		/// </param>
		/// <param name="">trigger
		/// the trigger to insert
		/// </param>
		/// <returns> the number of rows updated
		/// </returns>
		
		int UpdateBlobTrigger(IDbConnection conn, Trigger trigger);

		/// <summary> <p>
		/// Check whether or not a trigger exists.
		/// </p>
		/// 
		/// </summary>
		/// <param name="">conn
		/// the DB Connection
		/// </param>
		/// <param name="">triggerName
		/// the name of the trigger
		/// </param>
		/// <param name="">groupName
		/// the group containing the trigger
		/// </param>
		/// <returns> the number of rows updated
		/// </returns>
		
		bool TriggerExists(IDbConnection conn, string triggerName, string groupName);

		/// <summary> <p>
		/// Update the state for a given trigger.
		/// </p>
		/// 
		/// </summary>
		/// <param name="">conn
		/// the DB Connection
		/// </param>
		/// <param name="">triggerName
		/// the name of the trigger
		/// </param>
		/// <param name="">groupName
		/// the group containing the trigger
		/// </param>
		/// <param name="">state
		/// the new state for the trigger
		/// </param>
		/// <returns> the number of rows updated
		/// </returns>
		
		int UpdateTriggerState(IDbConnection conn, string triggerName, string groupName, string state);

		/// <summary> <p>
		/// Update the given trigger to the given new state, if it is in the given
		/// old state.
		/// </p>
		/// 
		/// </summary>
		/// <param name="">conn
		/// the DB connection
		/// </param>
		/// <param name="">triggerName
		/// the name of the trigger
		/// </param>
		/// <param name="">groupName
		/// the group containing the trigger
		/// </param>
		/// <param name="">newState
		/// the new state for the trigger
		/// </param>
		/// <param name="">oldState
		/// the old state the trigger must be in
		/// </param>
		/// <returns> int the number of rows updated
		/// </returns>
		/// <throws>  SQLException </throws>
		
		int UpdateTriggerStateFromOtherState(IDbConnection conn, string triggerName, string groupName, string newState,
		                                     string oldState);

		/// <summary> <p>
		/// Update the given trigger to the given new state, if it is one of the
		/// given old states.
		/// </p>
		/// 
		/// </summary>
		/// <param name="">conn
		/// the DB connection
		/// </param>
		/// <param name="">triggerName
		/// the name of the trigger
		/// </param>
		/// <param name="">groupName
		/// the group containing the trigger
		/// </param>
		/// <param name="">newState
		/// the new state for the trigger
		/// </param>
		/// <param name="">oldState1
		/// one of the old state the trigger must be in
		/// </param>
		/// <param name="">oldState2
		/// one of the old state the trigger must be in
		/// </param>
		/// <param name="">oldState3
		/// one of the old state the trigger must be in
		/// </param>
		/// <returns> int the number of rows updated
		/// </returns>
		/// <throws>  SQLException </throws>
		
		int UpdateTriggerStateFromOtherStates(IDbConnection conn, string triggerName, string groupName, string newState,
		                                      string oldState1, string oldState2, string oldState3);

		/// <summary> <p>
		/// Update the all triggers to the given new state, if they are in one of
		/// the given old states AND its next fire time is before the given time.
		/// </p>
		/// 
		/// </summary>
		/// <param name="">conn
		/// the DB connection
		/// </param>
		/// <param name="">newState
		/// the new state for the trigger
		/// </param>
		/// <param name="">oldState1
		/// one of the old state the trigger must be in
		/// </param>
		/// <param name="">oldState2
		/// one of the old state the trigger must be in
		/// </param>
		/// <param name="">time
		/// the time before which the trigger's next fire time must be
		/// </param>
		/// <returns> int the number of rows updated
		/// </returns>
		/// <throws>  SQLException </throws>
		
		int UpdateTriggerStateFromOtherStatesBeforeTime(IDbConnection conn, string newState, string oldState1,
		                                                string oldState2, long time);

		/// <summary> <p>
		/// Update all triggers in the given group to the given new state, if they
		/// are in one of the given old states.
		/// </p>
		/// 
		/// </summary>
		/// <param name="">conn
		/// the DB connection
		/// </param>
		/// <param name="">groupName
		/// the group containing the trigger
		/// </param>
		/// <param name="">newState
		/// the new state for the trigger
		/// </param>
		/// <param name="">oldState1
		/// one of the old state the trigger must be in
		/// </param>
		/// <param name="">oldState2
		/// one of the old state the trigger must be in
		/// </param>
		/// <param name="">oldState3
		/// one of the old state the trigger must be in
		/// </param>
		/// <returns> int the number of rows updated
		/// </returns>
		/// <throws>  SQLException </throws>
		
		int UpdateTriggerGroupStateFromOtherStates(IDbConnection conn, string groupName, string newState, string oldState1,
		                                           string oldState2, string oldState3);

		/// <summary> <p>
		/// Update all of the triggers of the given group to the given new state, if
		/// they are in the given old state.
		/// </p>
		/// 
		/// </summary>
		/// <param name="">conn
		/// the DB connection
		/// </param>
		/// <param name="">groupName
		/// the group containing the triggers
		/// </param>
		/// <param name="">newState
		/// the new state for the trigger group
		/// </param>
		/// <param name="">oldState
		/// the old state the triggers must be in
		/// </param>
		/// <returns> int the number of rows updated
		/// </returns>
		/// <throws>  SQLException </throws>
		
		int UpdateTriggerGroupStateFromOtherState(IDbConnection conn, string groupName, string newState, string oldState);

		/// <summary> <p>
		/// Update the states of all triggers associated with the given job.
		/// </p>
		/// 
		/// </summary>
		/// <param name="">conn
		/// the DB Connection
		/// </param>
		/// <param name="">jobName
		/// the name of the job
		/// </param>
		/// <param name="">groupName
		/// the group containing the job
		/// </param>
		/// <param name="">state
		/// the new state for the triggers
		/// </param>
		/// <returns> the number of rows updated
		/// </returns>
		
		int UpdateTriggerStatesForJob(IDbConnection conn, string jobName, string groupName, string state);

		/// <summary> <p>
		/// Update the states of any triggers associated with the given job, that
		/// are the given current state.
		/// </p>
		/// 
		/// </summary>
		/// <param name="">conn
		/// the DB Connection
		/// </param>
		/// <param name="">jobName
		/// the name of the job
		/// </param>
		/// <param name="">groupName
		/// the group containing the job
		/// </param>
		/// <param name="">state
		/// the new state for the triggers
		/// </param>
		/// <param name="">oldState
		/// the old state of the triggers
		/// </param>
		/// <returns> the number of rows updated
		/// </returns>
		
		int UpdateTriggerStatesForJobFromOtherState(IDbConnection conn, string jobName, string groupName, string state,
		                                            string oldState);

		/// <summary> <p>
		/// Delete all of the listeners associated with a given trigger.
		/// </p>
		/// 
		/// </summary>
		/// <param name="">conn
		/// the DB Connection
		/// </param>
		/// <param name="">triggerName
		/// the name of the trigger whose listeners will be deleted
		/// </param>
		/// <param name="">groupName
		/// the name of the group containing the trigger
		/// </param>
		/// <returns> the number of rows deleted
		/// </returns>
		
		int DeleteTriggerListeners(IDbConnection conn, string triggerName, string groupName);

		/// <summary> <p>
		/// Associate a listener with the given trigger.
		/// </p>
		/// 
		/// </summary>
		/// <param name="">conn
		/// the DB Connection
		/// </param>
		/// <param name="">trigger
		/// the trigger
		/// </param>
		/// <param name="">listener
		/// the name of the listener to associate with the trigger
		/// </param>
		/// <returns> the number of rows inserted
		/// </returns>
		
		int InsertTriggerListener(IDbConnection conn, Trigger trigger, string listener);

		/// <summary> <p>
		/// Select the listeners associated with a given trigger.
		/// </p>
		/// 
		/// </summary>
		/// <param name="">conn
		/// the DB Connection
		/// </param>
		/// <param name="">triggerName
		/// the name of the trigger
		/// </param>
		/// <param name="">groupName
		/// the group containing the trigger
		/// </param>
		/// <returns> array of <code>String</code> trigger listener names
		/// </returns>
		
		string[] SelectTriggerListeners(IDbConnection conn, string triggerName, string groupName);

		/// <summary> <p>
		/// Delete the simple trigger data for a trigger.
		/// </p>
		/// 
		/// </summary>
		/// <param name="">conn
		/// the DB Connection
		/// </param>
		/// <param name="">triggerName
		/// the name of the trigger
		/// </param>
		/// <param name="">groupName
		/// the group containing the trigger
		/// </param>
		/// <returns> the number of rows deleted
		/// </returns>
		
		int DeleteSimpleTrigger(IDbConnection conn, string triggerName, string groupName);

		/// <summary> <p>
		/// Delete the BLOB trigger data for a trigger.
		/// </p>
		/// 
		/// </summary>
		/// <param name="">conn
		/// the DB Connection
		/// </param>
		/// <param name="">triggerName
		/// the name of the trigger
		/// </param>
		/// <param name="">groupName
		/// the group containing the trigger
		/// </param>
		/// <returns> the number of rows deleted
		/// </returns>
		
		int DeleteBlobTrigger(IDbConnection conn, string triggerName, string groupName);

		/// <summary> <p>
		/// Delete the cron trigger data for a trigger.
		/// </p>
		/// 
		/// </summary>
		/// <param name="">conn
		/// the DB Connection
		/// </param>
		/// <param name="">triggerName
		/// the name of the trigger
		/// </param>
		/// <param name="">groupName
		/// the group containing the trigger
		/// </param>
		/// <returns> the number of rows deleted
		/// </returns>
		
		int DeleteCronTrigger(IDbConnection conn, string triggerName, string groupName);

		/// <summary> <p>
		/// Delete the base trigger data for a trigger.
		/// </p>
		/// 
		/// </summary>
		/// <param name="">conn
		/// the DB Connection
		/// </param>
		/// <param name="">triggerName
		/// the name of the trigger
		/// </param>
		/// <param name="">groupName
		/// the group containing the trigger
		/// </param>
		/// <returns> the number of rows deleted
		/// </returns>
		
		int DeleteTrigger(IDbConnection conn, string triggerName, string groupName);

		/// <summary> <p>
		/// Select the number of triggers associated with a given job.
		/// </p>
		/// 
		/// </summary>
		/// <param name="">conn
		/// the DB Connection
		/// </param>
		/// <param name="">jobName
		/// the name of the job
		/// </param>
		/// <param name="">groupName
		/// the group containing the job
		/// </param>
		/// <returns> the number of triggers for the given job
		/// </returns>
		
		int SelectNumTriggersForJob(IDbConnection conn, string jobName, string groupName);

		/// <summary> <p>
		/// Select the job to which the trigger is associated.
		/// </p>
		/// 
		/// </summary>
		/// <param name="">conn
		/// the DB Connection
		/// </param>
		/// <param name="">triggerName
		/// the name of the trigger
		/// </param>
		/// <param name="">groupName
		/// the group containing the trigger
		/// </param>
		/// <returns> the <code>{@link org.quartz.JobDetail}</code> object
		/// associated with the given trigger
		/// </returns>
		JobDetail SelectJobForTrigger(IDbConnection conn, string triggerName, string groupName, IClassLoadHelper loadHelper);

		/// <summary> <p>
		/// Select the stateful jobs which are referenced by triggers in the given
		/// trigger group.
		/// </p>
		/// 
		/// </summary>
		/// <param name="">conn
		/// the DB Connection
		/// </param>
		/// <param name="">groupName
		/// the trigger group
		/// </param>
		/// <returns> a List of Keys to jobs.
		/// </returns>
		
		IList SelectStatefulJobsOfTriggerGroup(IDbConnection conn, string groupName);

		/// <summary> <p>
		/// Select the triggers for a job
		/// </p>
		/// 
		/// </summary>
		/// <param name="">conn
		/// the DB Connection
		/// </param>
		/// <param name="">jobName
		/// the name of the trigger
		/// </param>
		/// <param name="">groupName
		/// the group containing the trigger
		/// </param>
		/// <returns> an array of <code>(@link org.quartz.Trigger)</code> objects
		/// associated with a given job.
		/// </returns>
		/// <throws>  SQLException </throws>
		
		Trigger[] SelectTriggersForJob(IDbConnection conn, string jobName, string groupName);

		/// <summary> <p>
		/// Select the triggers for a calendar
		/// </p>
		/// 
		/// </summary>
		/// <param name="">conn
		/// the DB Connection
		/// </param>
		/// <param name="">jobName
		/// the name of the trigger
		/// </param>
		/// <param name="">groupName
		/// the group containing the trigger
		/// </param>
		/// <returns> an array of <code>(@link org.quartz.Trigger)</code> objects
		/// associated with a given job.
		/// </returns>
		/// <throws>  SQLException </throws>
		
		Trigger[] SelectTriggersForCalendar(IDbConnection conn, string calName);

		/// <summary> <p>
		/// Select a trigger.
		/// </p>
		/// 
		/// </summary>
		/// <param name="">conn
		/// the DB Connection
		/// </param>
		/// <param name="">triggerName
		/// the name of the trigger
		/// </param>
		/// <param name="">groupName
		/// the group containing the trigger
		/// </param>
		/// <returns> the <code>{@link org.quartz.Trigger}</code> object
		/// </returns>
		
		Trigger SelectTrigger(IDbConnection conn, string triggerName, string groupName);

		/// <summary> <p>
		/// Select a trigger's JobDataMap.
		/// </p>
		/// 
		/// </summary>
		/// <param name="">conn
		/// the DB Connection
		/// </param>
		/// <param name="">triggerName
		/// the name of the trigger
		/// </param>
		/// <param name="">groupName
		/// the group containing the trigger
		/// </param>
		/// <returns> the <code>{@link org.quartz.JobDataMap}</code> of the Trigger,
		/// never null, but possibly empty.
		/// </returns>
		
		JobDataMap SelectTriggerJobDataMap(IDbConnection conn, string triggerName, string groupName);

		/// <summary> <p>
		/// Select a trigger' state value.
		/// </p>
		/// 
		/// </summary>
		/// <param name="">conn
		/// the DB Connection
		/// </param>
		/// <param name="">triggerName
		/// the name of the trigger
		/// </param>
		/// <param name="">groupName
		/// the group containing the trigger
		/// </param>
		/// <returns> the <code>{@link org.quartz.Trigger}</code> object
		/// </returns>
		
		string SelectTriggerState(IDbConnection conn, string triggerName, string groupName);

		/// <summary> <p>
		/// Select a trigger' status (state & next fire time).
		/// </p>
		/// 
		/// </summary>
		/// <param name="">conn
		/// the DB Connection
		/// </param>
		/// <param name="">triggerName
		/// the name of the trigger
		/// </param>
		/// <param name="">groupName
		/// the group containing the trigger
		/// </param>
		/// <returns> a <code>TriggerStatus</code> object, or null
		/// </returns>
		
		TriggerStatus SelectTriggerStatus(IDbConnection conn, string triggerName, string groupName);

		/// <summary> <p>
		/// Select the total number of triggers stored.
		/// </p>
		/// 
		/// </summary>
		/// <param name="">conn
		/// the DB Connection
		/// </param>
		/// <returns> the total number of triggers stored
		/// </returns>
		
		int SelectNumTriggers(IDbConnection conn);

		/// <summary> <p>
		/// Select all of the trigger group names that are stored.
		/// </p>
		/// 
		/// </summary>
		/// <param name="">conn
		/// the DB Connection
		/// </param>
		/// <returns> an array of <code>String</code> group names
		/// </returns>
		
		string[] SelectTriggerGroups(IDbConnection conn);

		/// <summary> <p>
		/// Select all of the triggers contained in a given group.
		/// </p>
		/// 
		/// </summary>
		/// <param name="">conn
		/// the DB Connection
		/// </param>
		/// <param name="">groupName
		/// the group containing the triggers
		/// </param>
		/// <returns> an array of <code>String</code> trigger names
		/// </returns>
		
		string[] SelectTriggersInGroup(IDbConnection conn, string groupName);

		/// <summary> <p>
		/// Select all of the triggers in a given state.
		/// </p>
		/// 
		/// </summary>
		/// <param name="">conn
		/// the DB Connection
		/// </param>
		/// <param name="">state
		/// the state the triggers must be in
		/// </param>
		/// <returns> an array of trigger <code>Key</code> s
		/// </returns>
		
		Key[] SelectTriggersInState(IDbConnection conn, string state);

		
		int InsertPausedTriggerGroup(IDbConnection conn, string groupName);

		
		int DeletePausedTriggerGroup(IDbConnection conn, string groupName);

		
		int DeleteAllPausedTriggerGroups(IDbConnection conn);

		
		bool IsTriggerGroupPaused(IDbConnection conn, string groupName);

		
		ISet SelectPausedTriggerGroups(IDbConnection conn);

		
		bool IsExistingTriggerGroup(IDbConnection conn, string groupName);

		//---------------------------------------------------------------------------
		// calendars
		//---------------------------------------------------------------------------

		/// <summary> <p>
		/// Insert a new calendar.
		/// </p>
		/// 
		/// </summary>
		/// <param name="">conn
		/// the DB Connection
		/// </param>
		/// <param name="">calendarName
		/// the name for the new calendar
		/// </param>
		/// <param name="">calendar
		/// the calendar
		/// </param>
		/// <returns> the number of rows inserted
		/// </returns>
		/// <throws>  IOException </throws>
		/// <summary>           if there were problems serializing the calendar
		/// </summary>
		
		int InsertCalendar(IDbConnection conn, string calendarName, ICalendar calendar);

		/// <summary> <p>
		/// Update a calendar.
		/// </p>
		/// 
		/// </summary>
		/// <param name="">conn
		/// the DB Connection
		/// </param>
		/// <param name="">calendarName
		/// the name for the new calendar
		/// </param>
		/// <param name="">calendar
		/// the calendar
		/// </param>
		/// <returns> the number of rows updated
		/// </returns>
		/// <throws>  IOException </throws>
		/// <summary>           if there were problems serializing the calendar
		/// </summary>
		
		int UpdateCalendar(IDbConnection conn, string calendarName, ICalendar calendar);

		/// <summary> <p>
		/// Check whether or not a calendar exists.
		/// </p>
		/// 
		/// </summary>
		/// <param name="">conn
		/// the DB Connection
		/// </param>
		/// <param name="">calendarName
		/// the name of the calendar
		/// </param>
		/// <returns> true if the trigger exists, false otherwise
		/// </returns>
		
		bool CalendarExists(IDbConnection conn, string calendarName);

		/// <summary> <p>
		/// Select a calendar.
		/// </p>
		/// 
		/// </summary>
		/// <param name="">conn
		/// the DB Connection
		/// </param>
		/// <param name="">calendarName
		/// the name of the calendar
		/// </param>
		/// <returns> the Calendar
		/// </returns>
		/// <throws>  ClassNotFoundException </throws>
		/// <summary>           if a class found during deserialization cannot be found be
		/// found
		/// </summary>
		/// <throws>  IOException </throws>
		/// <summary>           if there were problems deserializing the calendar
		/// </summary>
		
		ICalendar SelectCalendar(IDbConnection conn, string calendarName);

		/// <summary> <p>
		/// Check whether or not a calendar is referenced by any triggers.
		/// </p>
		/// 
		/// </summary>
		/// <param name="">conn
		/// the DB Connection
		/// </param>
		/// <param name="">calendarName
		/// the name of the calendar
		/// </param>
		/// <returns> true if any triggers reference the calendar, false otherwise
		/// </returns>
		
		bool CalendarIsReferenced(IDbConnection conn, string calendarName);

		/// <summary> <p>
		/// Delete a calendar.
		/// </p>
		/// 
		/// </summary>
		/// <param name="conn">The DB Connection</param>
		/// <param name="">calendarName
		/// the name of the trigger
		/// </param>
		/// <returns> the number of rows deleted
		/// </returns>
		int DeleteCalendar(IDbConnection conn, string calendarName);

		/// <summary> <p>
		/// Select the total number of calendars stored.
		/// </p>
		/// 
		/// </summary>
		/// <param name="conn">The DB Connection</param>
		/// <returns> the total number of calendars stored
		/// </returns>
		int SelectNumCalendars(IDbConnection conn);

		/// <summary> <p>
		/// Select all of the stored calendars.
		/// </p>
		/// 
		/// </summary>
		/// <param name="conn">The DB Connection</param>
		/// <returns> an array of <code>String</code> calendar names
		/// </returns>
		string[] SelectCalendars(IDbConnection conn);

		//---------------------------------------------------------------------------
		// trigger firing
		//---------------------------------------------------------------------------

		/// <summary> <p>
		/// Select the next time that a trigger will be fired.
		/// </p>
		/// 
		/// </summary>
		/// <param name="conn">The DB Connection</param>
		/// <returns> the next fire time, or 0 if no trigger will be fired
		/// </returns>
		DateTime SelectNextFireTime(IDbConnection conn);

		/// <summary> <p>
		/// Select the trigger that will be fired at the given fire time.
		/// </p>
		/// 
		/// </summary>
		/// <param name="conn">The DB Connection</param>
		/// <param name="">fireTime
		/// the time that the trigger will be fired
		/// </param>
		/// <returns> a <code>{@link org.quartz.utils.Key}</code> representing the
		/// trigger that will be fired at the given fire time, or null if no
		/// trigger will be fired at that time
		/// </returns>
		Key SelectTriggerForFireTime(IDbConnection conn, DateTime fireTime);

		/// <summary> <p>
		/// Insert a fired trigger.
		/// </p>
		/// 
		/// </summary>
		/// <param name="conn">The DB Connection</param>
		/// <param name="">trigger
		/// the trigger
		/// </param>
		/// <param name="">state
		/// the state that the trigger should be stored in
		/// </param>
		/// <returns> the number of rows inserted
		/// </returns>
		int InsertFiredTrigger(IDbConnection conn, Trigger trigger, string state, JobDetail jobDetail);

		/// <summary> <p>
		/// Select the states of all fired-trigger records for a given trigger, or
		/// trigger group if trigger name is <code>null</code>.
		/// </p>
		/// 
		/// </summary>
		/// <param name="conn">The DB Connection</param>
		/// <returns> a List of FiredTriggerRecord objects.
		/// </returns>
		IList SelectFiredTriggerRecords(IDbConnection conn, string triggerName, string groupName);

		/// <summary> <p>
		/// Select the states of all fired-trigger records for a given job, or job
		/// group if job name is <code>null</code>.
		/// </p>
		/// 
		/// </summary>
		/// <param name="conn">The DB Connection</param>
		/// <returns> a List of FiredTriggerRecord objects.
		/// </returns>
		IList SelectFiredTriggerRecordsByJob(IDbConnection conn, string jobName, string groupName);

		/// <summary> <p>
		/// Select the states of all fired-trigger records for a given scheduler
		/// instance.
		/// </p>
		/// 
		/// </summary>
		/// <param name="conn">The DB Connection</param>
		/// <returns> a List of FiredTriggerRecord objects.
		/// </returns>
		IList SelectInstancesFiredTriggerRecords(IDbConnection conn, string instanceName);

		/// <summary> <p>
		/// Delete a fired trigger.
		/// </p>
		/// 
		/// </summary>
		/// <param name="conn">The DB Connection</param>
		/// <param name="">entryId
		/// the fired trigger entry to delete
		/// </param>
		/// <returns> the number of rows deleted
		/// </returns>
		int DeleteFiredTrigger(IDbConnection conn, string entryId);

		/// <summary> <p>
		/// Get the number instances of the identified job currently executing.
		/// </p>
		/// 
		/// </summary>
		/// <param name="conn">The DB Connection</param>
		/// <returns> the number instances of the identified job currently executing.
		/// </returns>
		int SelectJobExecutionCount(IDbConnection conn, string jobName, string jobGroup);

		/// <summary> <p>
		/// Insert a scheduler-instance state record.
		/// </p>
		/// 
		/// </summary>
		/// <param name="conn">The DB Connection</param>
		/// <returns> the number of inserted rows.
		/// </returns>
		int InsertSchedulerState(IDbConnection conn, string instanceId, long checkInTime, long interval, string recoverer);

		/// <summary> <p>
		/// Delete a scheduler-instance state record.
		/// </p>
		/// 
		/// </summary>
		/// <param name="conn">The DB Connection</param>
		/// <returns> the number of deleted rows.
		/// </returns>
		int DeleteSchedulerState(IDbConnection conn, string instanceId);


		/// <summary> <p>
		/// Update a scheduler-instance state record.
		/// </p>
		/// 
		/// </summary>
		/// <param name="conn">The DB Connection</param>
		/// <returns> the number of updated rows.
		/// </returns>
		int UpdateSchedulerState(IDbConnection conn, string instanceId, long checkInTime);

		/// <summary> <p>
		/// A List of all current <code>SchedulerStateRecords</code>.
		/// </p>
		/// 
		/// <p>
		/// If instanceId is not null, then only the record for the identified
		/// instance will be returned.
		/// </p>
		/// 
		/// </summary>
		/// <param name="conn">The DB Connection</param>
		IList SelectSchedulerStateRecords(IDbConnection conn, string instanceId);
	}

	// EOF
}