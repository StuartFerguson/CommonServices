@email
Feature: SendEmail
	In order to communicate with external users
	As a sender
	I want to be able to send out emails

Background: 
	Given the messaging service is running

Scenario: Send Email
	Given My from email address is 'golfhandicapping@btinternet.com'
	And I want to send an email to 'stuart_ferguson1@btinternet.com'
	And I have the message 'Test Email Message'
	And I have a message subject 'Test Subject'
	When I send the message
	Then the result should indicate the message was sent
