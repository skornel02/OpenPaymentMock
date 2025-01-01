import { useMemo } from "react";
import {
    MantineReactTable,
    MRT_Row,
    MRT_TableOptions,
    useMantineReactTable,
    type MRT_ColumnDef,
} from 'mantine-react-table';
import { SchemaPartnerShortDto } from "@/lib/openpaymentmock-backend";
import { client, useBackendQuery } from "@/lib/openpaymentmock-client";
import { useAuthentication } from "@/contexts/authentication-context";
import { Button } from "@/components/ui/button";
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from "@radix-ui/react-tooltip";
import { Trash } from "lucide-react";
import PartnerKeys from "./components/partner-keys";

export default function PartnersPage() {
    const { apiKey } = useAuthentication();

    const {data, isLoading, isValidating, error, mutate} = useBackendQuery('/api/partners', {
        headers: {
            "X-Api-Key": apiKey
        }
    })

    const columns = useMemo<MRT_ColumnDef<SchemaPartnerShortDto>[]>(() => {
        const columns: MRT_ColumnDef<SchemaPartnerShortDto>[] = [
            {
                accessorKey: 'id',
                header: 'Id',
                enableEditing: false,
            },
            {
                accessorKey: 'name',
                header: 'Name',
                mantineEditTextInputProps: {
                    type: 'text',
                    required: true,
                },
            },
        ];

        return columns;
    }, []);

    //CREATE action
    const handleCreateUser: MRT_TableOptions<SchemaPartnerShortDto>['onCreatingRowSave'] = async ({
        values,
        exitCreatingMode,
    }) => {
        console.log('Creating user', values);

        try {
            await client.POST("/api/partners", {
                headers: {
                    "X-Api-Key": apiKey
                },
                body: {
                    name: values.name,
                }
            });

            exitCreatingMode();
            mutate();
        } catch (error) {
            console.error(error);
        }
    };

    const openDeleteConfirmModal = async (row: MRT_Row<SchemaPartnerShortDto>) => {
        if (confirm('Are you sure you want to delete this user?')) {
            await client.DELETE("/api/partners/{id}", {
                headers: {
                    "X-Api-Key": apiKey
                },
                params: {
                    path: {
                        id: row.original.id ?? "-"
                    }
                }
            });

            mutate();
        }
    }

    const table = useMantineReactTable({
        columns,
        data: data ?? [],
        getRowId: (row) => row.id,
        enableEditing: true,
        state: {
            isLoading,
            showProgressBars: isValidating,
            showAlertBanner: error !== undefined,
        },
        mantineToolbarAlertBannerProps: error
            ? { color: 'red', children: 'Error loading data' }
            : undefined,
        createDisplayMode: 'row',
        editDisplayMode: undefined,
        renderTopToolbarCustomActions: ({ table }) => (
            <div className="flex flex-row gap-2">
                <Button
                    variant="default"
                    onClick={() => {
                        table.setCreatingRow(true);
                    }}
                >
                    Add Partner
                </Button>
                <Button
                    variant="secondary"
                    onClick={() => {
                        mutate();
                    }}>
                    Refresh data
                </Button>
            </div>
        ),
        positionActionsColumn: 'last',
        renderRowActions: ({ row }) => (
            <div className="flex flex-row gap-2">
                {/* <TooltipProvider>
                    <Tooltip>
                        <TooltipTrigger>
                            <Button
                                variant="outline"
                                onClick={() => table.setEditingRow(row)}
                            >
                                <Edit />
                            </Button>
                        </TooltipTrigger>
                        <TooltipContent>
                            Edit
                        </TooltipContent>
                    </Tooltip>
                </TooltipProvider> */}
                <TooltipProvider>
                    <Tooltip>
                        <TooltipTrigger>
                            <Button className="border-red-500"
                                variant="outline"
                                onClick={() => openDeleteConfirmModal(row)}
                            >
                                <Trash className="text-red-600" />
                            </Button>
                        </TooltipTrigger>
                        <TooltipContent>
                            Delete
                        </TooltipContent>
                    </Tooltip>
                </TooltipProvider>
            </div>
        ),
        onCreatingRowSave: handleCreateUser,
        renderDetailPanel: ({ row }) => {
            return <>
                <PartnerKeys partner={row.original}/>
            </>;
        },
    });

    return <MantineReactTable table={table} />;
}
