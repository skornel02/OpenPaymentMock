# Open Payment Mock

OpenPaymentMock is a third-party payment provider designed to simulate payment processing in a secure and reliable manner. It is ideal for developers and teams who need a lightweight, mock implementation of payment gateways for testing, integration, or prototyping without the complexity or costs of interacting with real payment providers.

## State Machines

### Payment attempt

```mermaid
stateDiagram-v2
	NotAttempted --> Started : Started
	Started --> Succeeded : Success
	Started --> PaymentError : Cancel / Function
	Started --> PaymentError : Issue / Function
	Started --> TimedOut : Timeout
	Started --> BankVerificationRequired : BankVerification
	BankVerificationRequired --> Succeeded : Success
	BankVerificationRequired --> PaymentError : Issue / Function
[*] --> NotAttempted
```

### Payment situation

```mermaid
stateDiagram-v2
	Created --> Processing : Started
	Created --> Cancelled : Cancel
	Processing --> Succeeded : Success
	Processing --> Failed : Failed
	Processing --> Cancelled : Cancel
	Succeeded --> Refunded : Refund
[*] --> Created
```

## Contributing

We welcome contributions! 