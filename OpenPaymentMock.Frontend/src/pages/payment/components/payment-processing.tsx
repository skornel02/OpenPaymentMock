import { SchemaCurrentPaymentAttemptDto } from '@/lib/openpaymentmock-backend';
import { CreditCard } from 'lucide-react';
import { useCallback, useEffect, useMemo, useState } from 'react';
import { faker } from '@faker-js/faker';
import Cards, { Focused } from 'react-credit-cards-2';
import 'react-credit-cards-2/dist/es/styles-compiled.css';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Button } from '@/components/ui/button';
import { useInterval } from '@mantine/hooks';

interface CardInformation {
  number: string;
  expiry: string;
  cvc: string;
  name: string;
  focus: Focused;
}

const paymentTimeoutSeconds = 300;

const emptyCard: CardInformation = {
  number: '',
  expiry: '',
  cvc: '',
  name: '',
  focus: '',
};

function randomCard(): CardInformation {
  return {
    number: faker.finance.creditCardNumber('[4-6]###############'),
    expiry:
      (faker.date.future().getUTCMonth() + 1).toString().padStart(2, '0') +
      '/' +
      faker.date.future({ years: 10 }).getFullYear().toString().slice(-2),
    cvc: faker.finance.creditCardCVV(),
    focus: '',
    name: faker.person.fullName(),
  };
}

export default function PaymentProcessing({
  currentStatus,
  completePaymentSuccessfully,
  completePaymentCancelled,
  completePaymentFailed,
}: {
  currentStatus: SchemaCurrentPaymentAttemptDto;
  completePaymentSuccessfully: () => void;
  completePaymentCancelled: () => void;
  completePaymentFailed: (error: string) => void;
}) {
  const readOnlyForm = useMemo(
    () =>
      currentStatus.paymentSituation.paymentOptions.generateRandomCardDetails,
    [currentStatus],
  );

  const [card, setCard] = useState<CardInformation>(() =>
    currentStatus.paymentSituation.paymentOptions.generateRandomCardDetails
      ? randomCard()
      : emptyCard,
  );

  const [timeRemaining, setTimeRemaining] = useState<number>(300);

  const updateTimer = useCallback( () => {
    const now = new Date().getTime();
    const startedAt = new Date(currentStatus.createdAt).getTime();
    const elapsed = now - startedAt;

    setTimeRemaining(paymentTimeoutSeconds - elapsed / 1000);
  }, [currentStatus]);

  const timeoutTimer = useInterval(updateTimer, 1000);

  useEffect(() => {
    timeoutTimer.start();
    return timeoutTimer.stop;
  }, [timeoutTimer])

  return (
    <>
      <div className="flex flex-col items-center  justify-start lg:grid lg:grid-flow-col lg:items-start min-h-screen bg-gray-100 dark:bg-gray-950 py-10 gap-4">
        <div className="bg-white dark:bg-gray-800 p-8 rounded-lg shadow-lg w-full max-w-md">
          <div className="flex flex-col items-center justify-center space-y-6 mb-4">
            <div className="bg-blue-300 rounded-full p-4">
              <CreditCard className="h-12 w-12 text-white" />
            </div>
            <h1 className="text-2xl font-bold text-gray-900 dark:text-gray-50">
              Payment
            </h1>
          </div>

          <div className="space-y-2 text-center">
            <p className="text-gray-500 dark:text-gray-400">
              Your payment of{' '}
              <span className="font-medium">
                {currentStatus.paymentSituation.amount} (
                {currentStatus.paymentSituation.currency})
              </span>{' '}
              to{' '}
              <span className="font-medium">
                {currentStatus.paymentSituation.partnerName}
              </span>
            </p>

            <p className="text-gray-500 dark:text-gray-400">
              Payment attempt started at:{' '}
              <span className="font-medium">
                {new Date(currentStatus.createdAt).toLocaleString()}
              </span>
            </p>

            <p className="text-gray-500 dark:text-gray-400">
              Time remaining:{' '}
              <span className="font-medium">
                {timeRemaining.toFixed(0)} seconds
              </span>
            </p>
          </div>
        </div>

        <div className="bg-white dark:bg-gray-800 p-8 rounded-lg shadow-lg w-full max-w-md min-w-[340px]">
          <div className="flex flex-col items-center justify-center space-y-6 mb-4">
            <h1 className="text-2xl font-bold text-gray-900 dark:text-gray-50">
              Successful payment
            </h1>
          </div>
          <div className="flex flex-col items-center justify-center space-y-6">
            <Cards
              number={card.number}
              name={card.name}
              expiry={card.expiry}
              cvc={card.cvc}
              focused={card.focus}
            />
            <div className="grid w-full max-w-sm items-center gap-1.5">
              <Label htmlFor="number">Card number</Label>
              <Input
                type="number"
                id="number"
                placeholder="Card number"
                value={card.number}
                onChange={(e) => setCard({ ...card, number: e.target.value })}
                onFocus={() => setCard({ ...card, focus: 'number' })}
                disabled={readOnlyForm}
              />
            </div>
            <div className="grid w-full max-w-sm items-center gap-1.5">
              <Label htmlFor="name">Name</Label>
              <Input
                type="text"
                id="name"
                placeholder="Name"
                value={card.name}
                onChange={(e) => setCard({ ...card, name: e.target.value })}
                onFocus={() => setCard({ ...card, focus: 'name' })}
                disabled={readOnlyForm}
              />
            </div>
            <div className="grid w-full max-w-sm items-center gap-1.5">
              <Label htmlFor="expiry">Expiry</Label>
              <Input
                type="text"
                id="expiry"
                placeholder="MM/YY"
                value={card.expiry}
                onChange={(e) => setCard({ ...card, expiry: e.target.value })}
                onFocus={() => setCard({ ...card, focus: 'expiry' })}
                disabled={readOnlyForm}
              />
            </div>
            <div className="grid w-full max-w-sm items-center gap-1.5">
              <Label htmlFor="cvc">CVC</Label>
              <Input
                type="number"
                id="cvc"
                placeholder="CVC"
                value={card.cvc}
                onChange={(e) => setCard({ ...card, cvc: e.target.value })}
                onFocus={() => setCard({ ...card, focus: 'cvc' })}
                disabled={readOnlyForm}
              />
            </div>
            <div className="flex flex-col gap-2 w-full">
              <Button
                className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded"
                onClick={() => {
                  setCard(randomCard());
                }}
              >
                Random
              </Button>
              <Button
                className="bg-green-500 hover:bg-green-700 text-white font-bold py-2 px-4 rounded"
                onClick={completePaymentSuccessfully}
              >
                Pay successfully
              </Button>
              <Button
                className="bg-blue-600 hover:bg-green-700 text-white font-bold py-2 px-4 rounded"
                onClick={() => {}}
                disabled
              >
                Pay with bank verification
              </Button>
            </div>
          </div>
        </div>

        <div className="bg-white dark:bg-gray-800 p-8 rounded-lg shadow-lg w-full max-w-md">
          <div className="flex flex-col items-center justify-center space-y-6 mb-4">
            <h1 className="text-2xl font-bold text-gray-900 dark:text-gray-50">
              Transient issues
            </h1>
          </div>

          <div className="space-y-2 text-center">
            <p className="text-gray-500 dark:text-gray-400 my-2">
              When you use these options the payment will not fail immediately.
              You will have the chance to pay again until the timeout is
              reached.
            </p>
          </div>

          <div className="flex flex-col items-center justify-center space-y-6">
            <div className="flex flex-col gap-2 w-full">
              <Button
                className="bg-red-600 hover:bg-red-700 text-white font-bold py-2 px-4 rounded"
                onClick={() => {completePaymentFailed('ERR01: Not enough funds');}}
              >
                Not enough funds
              </Button>
              <Button
                className="bg-red-600 hover:bg-red-700 text-white font-bold py-2 px-4 rounded"
                onClick={() => {completePaymentFailed('ERR02: Card blocked');}}
              >
                Card blocked
              </Button>
              <Button
                className="bg-red-600 hover:bg-red-700 text-white font-bold py-2 px-4 rounded"
                onClick={() => {completePaymentFailed('ERR03: Card expired');}}
              >
                Card expired
              </Button>
              <Button
                className="bg-red-600 hover:bg-red-700 text-white font-bold py-2 px-4 rounded"
                onClick={() => {completePaymentFailed('ERR04: Card not supported');}}
              >
                Card not supported
              </Button>
            </div>
          </div>
        </div>

        <div className="bg-white dark:bg-gray-800 p-8 rounded-lg shadow-lg w-full max-w-md">
          <div className="flex flex-col items-center justify-center space-y-6 mb-4">
            <h1 className="text-2xl font-bold text-gray-900 dark:text-gray-50">
              Payment failed
            </h1>
          </div>

          <div className="space-y-2 text-center">
            <p className="text-gray-500 dark:text-gray-400 my-2">
              This will permanently cancel the payment.
            </p>
          </div>

          <div className="flex flex-col items-center justify-center space-y-6">
            <div className="flex flex-col gap-2 w-full">
              <Button
                className="bg-red-600 hover:bg-red-700 text-white font-bold py-2 px-4 rounded"
                onClick={completePaymentCancelled}
              >
                Cancel payment
              </Button>
            </div>
          </div>
        </div>
      </div>
    </>
  );
}
