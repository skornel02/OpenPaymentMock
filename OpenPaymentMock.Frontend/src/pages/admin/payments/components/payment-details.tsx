import { useAuthentication } from '@/contexts/authentication-context';
import { SchemaPaymentSituationDetailsDto } from '@/lib/openpaymentmock-backend';
import { useBackendQuery } from '@/lib/openpaymentmock-client';
import { DefaultMantineColor, Timeline } from '@mantine/core';
import { useMemo } from 'react';

export default function PaymentDetails({
  payment,
}: {
  payment: SchemaPaymentSituationDetailsDto;
}) {
  const { apiKey } = useAuthentication();

  const { data: attempts, isLoading: attemptsIsLoading } = useBackendQuery(
    '/api/payments/{paymentId}/attempts',
    {
      params: {
        path: {
          paymentId: payment.id,
        },
      },
      headers: {
        'X-Api-Key': apiKey,
      },
    },
  );

  const attemptsSorted = useMemo(() => {
    return attempts?.toSorted(
      (a, b) =>
        new Date(a.createdAt).getTime() - new Date(b.createdAt).getTime(),
    );
  }, [attempts]);

  const lastFinishedAttempt = useMemo(() => {
    if (!attemptsSorted) return undefined;

    if (attemptsSorted.length < 2) {
      return attemptsSorted.length;
    }

    if (
      attemptsSorted[attemptsSorted.length - 1].status === 'Started' ||
      attemptsSorted[attemptsSorted.length - 1].status === 'NotAttempted'
    ) {
      return attemptsSorted.length - 1;
    }

    return attemptsSorted.length;
  }, [attemptsSorted]);

  return (
    <div>
      <div className="p-2">
        <h1 className="text-xl mb-4">Payment attempts</h1>

        {attemptsIsLoading && <div>Loading...</div>}

        {attemptsSorted && (
          <Timeline active={lastFinishedAttempt}>
            {attemptsSorted.map((attempt, i) => {
              let color: DefaultMantineColor = 'gray';

              switch (attempt.status) {
                case 'Started':
                  color = 'blue';
                  break;
                case 'PaymentError':
                case 'TimedOut':
                  color = 'red';
                  break;
                case 'Succeeded':
                  color = 'green';
                  break;
                case 'BankVerificationRequired':
                  color = 'orange';
                  break;
              }

              return (
                <Timeline.Item
                  key={attempt.id}
                  color={color}
                  lineVariant={
                    i == (lastFinishedAttempt ?? 0) - 1 ? 'dashed' : 'solid'
                  }
                >
                  <h1 className="font-bold">Attempt #{i + 1}</h1>
                  <p>
                    {attempt.status}{' '}
                    {attempt.paymentError && (
                      <span className="font-semibold">
                        {' '}
                        - {attempt.paymentError}
                      </span>
                    )}
                  </p>
                  <p className="italic">
                    {new Date(attempt.createdAt).toLocaleString()}
                  </p>
                </Timeline.Item>
              );
            })}
          </Timeline>
        )}
      </div>
    </div>
  );
}
