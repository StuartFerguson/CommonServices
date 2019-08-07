@sms
Feature: SendSMS
	In order to communicate with external users
	As a sender
	I want to be able to send out sms messages

Background: 
	Given the messaging service is running

Scenario: Send SMS
	Given My from mobile number '07777777777'
	And I want to send an sms to '07777777778'
	And I have the message 'Test SMS Message'
	When I send the message
	Then the result should indicate the message was sent
