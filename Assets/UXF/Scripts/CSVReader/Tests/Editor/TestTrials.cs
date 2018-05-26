﻿using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace UXF.Tests {

	public class TestTrials
	{

        GameObject gameObject;
        Session session;
        FileIOManager fileIOManager;
        SessionLogger sessionLogger;

        [SetUp]
        public void SetUp()
        {
            gameObject = new GameObject();
            fileIOManager = gameObject.AddComponent<FileIOManager>();
            sessionLogger = gameObject.AddComponent<SessionLogger>();
            session = gameObject.AddComponent<Session>();

            session.AttachReferences(
                fileIOManager,
                sessionLogger
            );

            sessionLogger.AttachReferences(
                fileIOManager,
                session
            );

            fileIOManager.debug = true;
            fileIOManager.Begin();

            string experimentName = "unit_test";
            string ppid = "test_trials";
            session.Begin(experimentName, ppid, "example_output");
            session.customHeaders.Add("observation");

			// generate trials
			session.CreateBlock(2);
            session.CreateBlock(3);
        }

        [TearDown]
        public void TearDown()
        {
            session.End();
            fileIOManager.Quit();
            GameObject.DestroyImmediate(gameObject);
        }

        [Test]
        public void RunTrials()
        {   
            int i = 0;
            foreach (var trial in session.trials)
            {
                trial.Begin();
                trial.result["observation"] = ++i;

                Assert.Throws<KeyNotFoundException>(
                    delegate { trial.result["other_observation"] = "something"; }
                );

                Assert.AreSame(trial, session.currentTrial);
                Assert.AreEqual(trial.number, session.currentTrialNum);

                trial.End();
            }

            i = 0;
            foreach (var trial in session.trials)
            {
                Assert.AreEqual(trial.result["observation"], ++i);
            }
        }

	}

}